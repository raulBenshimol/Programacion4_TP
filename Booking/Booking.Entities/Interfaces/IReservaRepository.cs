namespace Booking.Booking.Entities.Interfaces;

// Patrón Repository: Business lee y guarda reservas sin saber qué base hay abajo.
// Está en Entities (el Domain del diagrama) para que Business y Data dependan de Entities y no entre ellas.
public interface IReservaRepository
{
    // Lanza AsientoOcupadoException o SolicitudDuplicadaException.
    Task Guardar(Reserva reserva, CancellationToken ct = default);

    Task<Reserva?> ObtenerPorId(Guid id, CancellationToken ct = default);

    // Lanza ConcurrenciaReservaException.
    Task Actualizar(Reserva reserva, CancellationToken ct = default);

    // TODO(contrato): no está en el diagrama; para la idempotencia y GET /api/solicitudes/{solicitudId}/estado.
    Task<Reserva?> ObtenerPorSolicitudId(Guid solicitudId, CancellationToken ct = default);

    // TODO(contrato): no está en el diagrama; para GET /api/eventos/{eventoId}/asientos-ocupados.
    Task<IReadOnlyList<string>> ObtenerAsientosOcupados(int eventoId, CancellationToken ct = default);

    // TODO(diagrama): no está en el diagrama; vence las Retenidas con vencimiento <= ahora y las devuelve.
    Task<IReadOnlyList<Reserva>> VencerRetenidas(DateTime ahora, CancellationToken ct = default);
}
