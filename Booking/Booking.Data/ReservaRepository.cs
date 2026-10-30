using Booking.Booking.Entities;
using Booking.Booking.Entities.Booking.Entities.Enums;
using Booking.Booking.Entities.Excepciones;
using Booking.Booking.Entities.Interfaces;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Booking.Booking.Data;

// Implementa IReservaRepository con EF Core y traduce los errores de SQLite a excepciones
// del dominio, para que Business no conozca EF Core ni SqliteException.
public class ReservaRepository(BookingDbContext context) : IReservaRepository
{
    // SQLITE_CONSTRAINT_UNIQUE. SQLite informa las columnas del índice que falló, no su nombre.
    private const int ErrorRestriccionUnica = 2067;
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
            // Se desengancha para poder seguir usando el DbContext (ej. guardarla como Rechazada).
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
        // Concurrencia optimista: el UPDATE compara la Version leída y guarda la siguiente.
        // El original se fija a mano por si la reserva vino sin tracking.
        var entry = context.Reservas.Update(reserva);
        entry.Property(r => r.Version).OriginalValue = reserva.Version;
        reserva.Version++;
        try
        {
            await context.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            entry.State = EntityState.Detached;
            reserva.Version--;
            throw new ConcurrenciaReservaException(reserva.Id);
        }
    }

    public Task<Reserva?> ObtenerPorSolicitudId(Guid solicitudId, CancellationToken ct = default) =>
        context.Reservas.FirstOrDefaultAsync(r => r.SolicitudId == solicitudId, ct);

    // Mismo criterio que el índice parcial: una Retenida ya vencida ocupa hasta que el job la vence.
    public async Task<IReadOnlyList<string>> ObtenerAsientosOcupados(Guid eventoId, CancellationToken ct = default) =>
        await context.Reservas
            .AsNoTracking()
            .Where(r => r.EventoId == eventoId
                && (r.Estado == ReservationStatusEnum.Retenida || r.Estado == ReservationStatusEnum.Confirmada))
            .Select(r => r.AsientoId)
            .OrderBy(a => a)
            .ToListAsync(ct);

    // Un solo UPDATE atómico: SQLite admite un escritor a la vez, así que con varias réplicas cada
    // reserva se vence una sola vez. Sube Version para que un cobro en curso falle al confirmar.
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
