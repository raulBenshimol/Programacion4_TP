namespace Booking.Booking.Entities.Interfaces;

// Patrón Repository: Business pide y guarda reservas a través de esta interfaz,
// sin saber que abajo hay EF Core y SQLite. La implementación está en Booking.Data.
// Está en Entities (el Domain del diagrama) para que Business y Data dependan de
// Entities y no entre ellas.
public interface IReservaRepository
{
    // Lanza AsientoOcupadoException o SolicitudDuplicadaException si la base
    // rechaza el alta por alguno de sus índices únicos.
    Task Guardar(Reserva reserva, CancellationToken ct = default);

    Task<Reserva?> ObtenerPorId(Guid id, CancellationToken ct = default);

    // Lanza ConcurrenciaReservaException si otro proceso la modificó antes.
    Task Actualizar(Reserva reserva, CancellationToken ct = default);

    // TODO(contrato): no está en el diagrama. Para la idempotencia y para el
    // GET /api/solicitudes/{solicitudId}/estado que consulta Availability.
    Task<Reserva?> ObtenerPorSolicitudId(Guid solicitudId, CancellationToken ct = default);

    // TODO(contrato): no está en el diagrama. Para el
    // GET /api/eventos/{eventoId}/asientos-ocupados que necesita Availability.
    Task<IReadOnlyList<string>> ObtenerAsientosOcupados(Guid eventoId, CancellationToken ct = default);

    // TODO(diagrama): no está en el diagrama. Pasa a Vencida, en una sola operación
    // atómica, todas las Retenidas cuyo vencimiento es anterior o igual a "ahora", y devuelve
    // las que cambió. Lo usa ExpirarReservaJob.
    Task<IReadOnlyList<Reserva>> VencerRetenidas(DateTime ahora, CancellationToken ct = default);
}
