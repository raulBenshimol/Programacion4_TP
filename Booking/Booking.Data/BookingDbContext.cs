using Booking.Booking.Entities;
using Booking.Booking.Entities.Booking.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace Booking.Booking.Data;

// Base propia de Booking (microservicios): Availability consulta por la API REST, no lee esta base.
public class BookingDbContext(DbContextOptions<BookingDbContext> options) : DbContext(options)
{
    public const string IndiceSolicitudUnica = "UX_Reservas_SolicitudId";
    public const string IndiceAsientoActivo = "UX_Reservas_EventoId_AsientoId_Activo";

    public DbSet<Reserva> Reservas => Set<Reserva>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Fluent API en vez de atributos, para que Entities no dependa de EF Core.
        var reserva = modelBuilder.Entity<Reserva>();

        reserva.ToTable("Reservas");
        reserva.HasKey(r => r.Id);

        reserva.Property(r => r.AsientoId).HasMaxLength(20).IsRequired();
        reserva.Property(r => r.UsuarioId).HasMaxLength(100).IsRequired();
        reserva.Property(r => r.MotivoRechazo).HasMaxLength(100);
        reserva.Property(r => r.Monto).HasPrecision(18, 2);

        // Estado como texto: la tabla se lee a simple vista y el filtro del índice queda legible.
        reserva.Property(r => r.Estado).HasConversion<string>().HasMaxLength(20);

        // Concurrencia optimista: EF agrega la Version leída al WHERE de cada UPDATE.
        reserva.Property(r => r.Version).IsConcurrencyToken();

        // Idempotencia: Service Bus puede entregar el mismo mensaje dos veces; el segundo choca acá.
        reserva.HasIndex(r => r.SolicitudId)
            .IsUnique()
            .HasDatabaseName(IndiceSolicitudUnica);

        // Concurrencia entre réplicas: índice único parcial, solo Retenida o Confirmada ocupan el asiento,
        // y la base garantiza que gane una sola. Vencida y Rechazada lo liberan sin borrar la fila.
        reserva.HasIndex(r => new { r.EventoId, r.AsientoId })
            .IsUnique()
            .HasFilter($"\"Estado\" IN ('{ReservationStatusEnum.Retenida}', '{ReservationStatusEnum.Confirmada}')")
            .HasDatabaseName(IndiceAsientoActivo);

        // Para que el job de vencimiento encuentre rápido las Retenidas vencidas.
        reserva.HasIndex(r => new { r.Estado, r.FechaVencimiento });
    }
}
