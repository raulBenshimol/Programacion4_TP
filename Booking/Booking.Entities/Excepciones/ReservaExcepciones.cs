namespace Booking.Booking.Entities.Excepciones;

// Excepciones propias que lanza el repositorio cuando la base rechaza una operación.
// Data traduce los errores de SQLite a estas excepciones para que Business
// pueda reaccionar sin conocer EF Core ni SqliteException (cada capa habla en sus términos).

// Ya hay una reserva Retenida o Confirmada para ese asiento de ese evento.
public class AsientoOcupadoException(Guid eventoId, string asientoId)
    : Exception($"El asiento {asientoId} del evento {eventoId} ya está ocupado.");

// Ya existe una reserva para esa solicitud (mensaje duplicado del broker).
public class SolicitudDuplicadaException(Guid solicitudId)
    : Exception($"Ya existe una reserva para la solicitud {solicitudId}.");

// Otro proceso modificó la reserva entre que la leímos y la quisimos guardar.
public class ConcurrenciaReservaException(Guid reservaId)
    : Exception($"La reserva {reservaId} fue modificada por otro proceso.");
