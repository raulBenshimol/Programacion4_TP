using Booking.Booking.Entities.Booking.Entities.Enums;

namespace Booking.Booking.Entities;

// Dominio anémico: la entidad es solo un contenedor de datos, sin comportamiento.
// Las reglas (cuándo se retiene, se confirma o se vence) viven en Booking.Business.
public class Reserva
{
    // Guid y no int como en el diagrama: el contrato expone reservaId como uuid,
    // y un Guid se puede generar en cualquier réplica sin pedírselo a la base.
    public Guid Id { get; set; }

    // TODO(contrato): no está en el diagrama. Hace falta para la idempotencia
    // (índice único: el mismo mensaje entregado dos veces no crea dos reservas)
    // y para que Availability consulte el estado por solicitudId.
    public Guid SolicitudId { get; set; }

    // TODO(contrato): no está en el diagrama. Un asiento "A15" se repite en cada
    // evento, así que el índice único de asiento ocupado es por (EventoId, AsientoId).
    public Guid EventoId { get; set; }

    // TODO(contrato): string como en SolicitudReserva.schema.json (ej. "A15"),
    // no int como en el diagrama.
    public string AsientoId { get; set; } = string.Empty;

    // TODO(contrato): string como en SolicitudReserva.schema.json, no int.
    public string UsuarioId { get; set; } = string.Empty;

    // Se usa el enum que ya tenía el grupo (mismos 4 estados del contrato).
    // Solo Retenida y Confirmada "ocupan" el asiento; Rechazada y Vencida lo dejan libre.
    public ReservationStatusEnum Estado { get; set; }

    // decimal porque es dinero (double tiene errores de redondeo). Se convierte
    // a double solo en el cliente gRPC, porque payment.proto usa double.
    // TODO(contrato): la SolicitudReserva no trae precio; por ahora sale de configuración.
    public decimal Monto { get; set; }

    // Siempre en UTC, para que todas las réplicas comparen igual el vencimiento.
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaVencimiento { get; set; }

    // Solo tiene valor cuando Estado es Rechazada (ver MotivosRechazo).
    public string? MotivoRechazo { get; set; }

    // Concurrencia optimista: un número que sube en cada UPDATE. SQLite no tiene
    // rowversion como SQL Server, así que lo incrementa ReservaRepository. Si dos
    // procesos modifican la misma reserva a la vez (ej. el job la vence mientras
    // Payment la confirma), el segundo en guardar falla en vez de pisar al primero.
    public long Version { get; set; }
}
