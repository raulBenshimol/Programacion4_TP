using Booking.Booking.Entities.Booking.Entities.Enums;

namespace Booking.Booking.Entities;

// Dominio anémico: solo datos, sin comportamiento. Las reglas viven en Booking.Business.
public class Reserva
{
    // Guid: el contrato expone reservaId como uuid y cualquier réplica lo puede generar.
    public Guid Id { get; set; }

    // TODO(contrato): no está en el diagrama; hace falta para la idempotencia y la consulta de Availability.
    public Guid SolicitudId { get; set; }

    // TODO(contrato): no está en el diagrama; el asiento se repite en cada evento.
    // int como lo publica Availability; SolicitudReserva.schema.json todavía dice Guid.
    public int EventoId { get; set; }

    // TODO(contrato): string como en SolicitudReserva.schema.json (ej. "A15"), no int como en el diagrama.
    public string AsientoId { get; set; } = string.Empty;

    // TODO(contrato): string como en SolicitudReserva.schema.json, no int.
    public string UsuarioId { get; set; } = string.Empty;

    public ReservationStatusEnum Estado { get; set; }

    // decimal y no double porque es dinero.
    // TODO(contrato): la SolicitudReserva no trae precio; por ahora sale de configuración.
    public decimal Monto { get; set; }

    // En UTC, para que todas las réplicas comparen igual el vencimiento.
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaVencimiento { get; set; }

    public string? MotivoRechazo { get; set; }

    // Token de concurrencia optimista: sube en cada UPDATE, así el segundo que guarda
    // la misma reserva falla en vez de pisar al primero.
    public long Version { get; set; }
}
