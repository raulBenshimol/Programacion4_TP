namespace Booking.Booking.Entities.Excepciones;

// Excepciones del dominio: Data traduce a estas los errores de SQLite, así Business no conoce EF Core.

public class AsientoOcupadoException(Guid eventoId, string asientoId)
    : Exception($"El asiento {asientoId} del evento {eventoId} ya está ocupado.");

public class SolicitudDuplicadaException(Guid solicitudId)
    : Exception($"Ya existe una reserva para la solicitud {solicitudId}.");

public class ConcurrenciaReservaException(Guid reservaId)
    : Exception($"La reserva {reservaId} fue modificada por otro proceso.");
