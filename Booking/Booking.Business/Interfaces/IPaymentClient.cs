namespace Booking.Booking.Business.Interfaces
{
    public enum ResultadoPago
    {
        Aprobado,
        Rechazado,
        NoDisponible
    }

    public interface IPaymentClient
    {
        Task<ResultadoPago> ProcesarPago(Guid solicitudId, Guid reservaId, decimal monto);
    }
}
