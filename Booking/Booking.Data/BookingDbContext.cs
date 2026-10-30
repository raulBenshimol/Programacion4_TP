using Booking.Booking.Entities;
using Booking.Booking.Entities.Booking.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace Booking.Booking.Data;

// Base de datos propia de Booking (estilo microservicios: ningún otro servicio
// lee esta base; Availability consulta el estado por la API REST).
// SQLite, igual que Availability: es un archivo (booking.db), no hace falta servidor.
public class BookingDbContext(DbContextOptions<BookingDbContext> options) : DbContext(options)
{
    public const string IndiceSolicitudUnica = "UX_Reservas_SolicitudId";
    public const string IndiceAsientoActivo = "UX_Reservas_EventoId_AsientoId_Activo";

    public DbSet<Reserva> Reservas => Set<Reserva>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Fluent API en vez de atributos en la entidad: así Entities no depende de EF Core.
        var reserva = modelBuilder.Entity<Reserva>();

        reserva.ToTable("Reservas");
        reserva.HasKey(r => r.Id);

        // SQLite guarda todo como TEXT y no hace cumplir los largos, pero quedan
        // documentados en el modelo (y servirían si se cambia de base).
        reserva.Property(r => r.AsientoId).HasMaxLength(20).IsRequired();
        reserva.Property(r => r.UsuarioId).HasMaxLength(100).IsRequired();
        reserva.Property(r => r.MotivoRechazo).HasMaxLength(100);
        reserva.Property(r => r.Monto).HasPrecision(18, 2);

        // El estado se guarda como texto ("Retenida") y no como número: la tabla se
        // entiende a simple vista y el filtro del índice de abajo queda legible.
        reserva.Property(r => r.Estado).HasConversion<string>().HasMaxLength(20);

        // Token de concurrencia: EF agrega "AND Version = <valor leído>" al WHERE de cada
        // UPDATE. Si no actualiza ninguna fila, otro proceso la cambió primero.
        reserva.Property(r => r.Version).IsConcurrencyToken();

        // IDEMPOTENCIA: una sola reserva por solicitud. Service Bus garantiza entrega
        // "al menos una vez", así que el mismo mensaje puede llegar dos veces (o a dos
        // réplicas); el segundo intento choca con este índice y no se duplica nada.
        reserva.HasIndex(r => r.SolicitudId)
            .IsUnique()
            .HasDatabaseName(IndiceSolicitudUnica);

        // CONCURRENCIA SOBRE EL MISMO ASIENTO: índice único PARCIAL (en SQLite, un
        // CREATE UNIQUE INDEX ... WHERE). Solo cuenta las reservas que ocupan el asiento
        // (Retenida o Confirmada). Con varias réplicas no alcanza con preguntar "¿está
        // libre?" y después insertar (dos réplicas pueden preguntar a la vez y las dos
        // ver "libre"); la base es la única que puede garantizar que una gane y la otra falle.
        // Cuando una reserva pasa a Vencida o Rechazada sale del filtro y el asiento
        // queda libre sin borrar la fila (queda el historial).
        // También sirve para la consulta de asientos ocupados por evento.
        reserva.HasIndex(r => new { r.EventoId, r.AsientoId })
            .IsUnique()
            .HasFilter($"\"Estado\" IN ('{ReservationStatusEnum.Retenida}', '{ReservationStatusEnum.Confirmada}')")
            .HasDatabaseName(IndiceAsientoActivo);

        // Para que el job de vencimiento (etapa 6) encuentre rápido las Retenidas vencidas.
        reserva.HasIndex(r => new { r.Estado, r.FechaVencimiento });
    }
}
