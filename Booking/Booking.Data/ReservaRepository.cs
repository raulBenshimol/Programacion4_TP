using Booking.Booking.Entities;
using Booking.Booking.Entities.Booking.Entities.Enums;
using Booking.Booking.Entities.Excepciones;
using Booking.Booking.Entities.Interfaces;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Booking.Booking.Data;

// Implementación de IReservaRepository con EF Core. Además de guardar y leer,
// traduce los errores de SQLite a excepciones del dominio, para que Business
// no tenga que conocer EF Core ni SqliteException.
public class ReservaRepository(BookingDbContext context) : IReservaRepository
{
    // Código extendido de SQLite para "UNIQUE constraint failed" (SQLITE_CONSTRAINT_UNIQUE).
    private const int ErrorRestriccionUnica = 2067;

    // SQLite no dice qué índice falló sino sus columnas, por ejemplo
    // "UNIQUE constraint failed: Reservas.SolicitudId". Se reconoce el índice por ellas.
    private const string ColumnasIndiceAsientoActivo = "Reservas.EventoId, Reservas.AsientoId";
    private const string ColumnasIndiceSolicitudUnica = "Reservas.SolicitudId";

    public async Task Guardar(Reserva reserva, CancellationToken ct = default)
    {
        context.Reservas.Add(reserva);
        try
        {
            await context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqliteException { SqliteExtendedErrorCode: ErrorRestriccionUnica } sqlite)
        {
            // Se desengancha lo que falló para que el mismo DbContext se pueda seguir
            // usando (por ejemplo, para guardar después la reserva como Rechazada).
            context.Entry(reserva).State = EntityState.Detached;

            if (sqlite.Message.Contains(ColumnasIndiceAsientoActivo))
                throw new AsientoOcupadoException(reserva.EventoId, reserva.AsientoId);
            if (sqlite.Message.Contains(ColumnasIndiceSolicitudUnica))
                throw new SolicitudDuplicadaException(reserva.SolicitudId);
            throw;
        }
    }

    public Task<Reserva?> ObtenerPorId(Guid id, CancellationToken ct = default) =>
        context.Reservas.FirstOrDefaultAsync(r => r.Id == id, ct);

    public async Task Actualizar(Reserva reserva, CancellationToken ct = default)
    {
        // El WHERE del UPDATE compara contra el valor ORIGINAL de Version (el que se leyó)
        // y se guarda el siguiente. Si la reserva vino sin tracking, Update toma el valor
        // actual como original, así que se fija explícitamente antes de incrementarlo.
        var entry = context.Reservas.Update(reserva);
        entry.Property(r => r.Version).OriginalValue = reserva.Version;
        reserva.Version++;
        try
        {
            await context.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            // La Version en la base ya no coincide: otro proceso la cambió primero.
            entry.State = EntityState.Detached;
            reserva.Version--;
            throw new ConcurrenciaReservaException(reserva.Id);
        }
    }

    public Task<Reserva?> ObtenerPorSolicitudId(Guid solicitudId, CancellationToken ct = default) =>
        context.Reservas.FirstOrDefaultAsync(r => r.SolicitudId == solicitudId, ct);

    // Mismo criterio que el índice parcial: ocupan el asiento las Retenidas y las
    // Confirmadas. Una Retenida que ya pasó su vencimiento sigue contando hasta que
    // el job la pase a Vencida, porque hasta ese momento la base también la bloquea.
    public async Task<IReadOnlyList<string>> ObtenerAsientosOcupados(Guid eventoId, CancellationToken ct = default) =>
        await context.Reservas
            .AsNoTracking()
            .Where(r => r.EventoId == eventoId
                && (r.Estado == ReservationStatusEnum.Retenida || r.Estado == ReservationStatusEnum.Confirmada))
            .Select(r => r.AsientoId)
            .OrderBy(a => a)
            .ToListAsync(ct);

    // Un solo UPDATE en vez de "leer las vencidas y actualizarlas una por una":
    // - Es atómico: SQLite deja escribir a un solo proceso a la vez, así que si dos
    //   réplicas corren el job a la vez la segunda espera y ya no ve esas filas como
    //   Retenida (cada reserva cambia UNA sola vez).
    // - Si el pago la confirmó un instante antes, ya no es Retenida y no se toca.
    // - RETURNING * devuelve las filas tal como quedaron, para loguear cuáles vencieron.
    // También sube Version, así que un cobro que estaba en curso sobre esa reserva
    // falla al confirmar en vez de pisar el Vencida.
    public async Task<IReadOnlyList<Reserva>> VencerRetenidas(DateTime ahora, CancellationToken ct = default) =>
        await context.Reservas
            .FromSql($"""
                UPDATE Reservas
                SET Estado = {nameof(ReservationStatusEnum.Vencida)}, Version = Version + 1
                WHERE Estado = {nameof(ReservationStatusEnum.Retenida)} AND FechaVencimiento <= {ahora}
                RETURNING *
                """)
            .AsNoTracking()
            .ToListAsync(ct);
}
