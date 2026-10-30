namespace Booking.Booking.Entities;

// Textos de rechazo pedidos por la consigna, centralizados para no escribirlos
// distinto en cada lugar (Availability los muestra tal cual).
public static class MotivosRechazo
{
    public const string AsientoOcupado = "Asiento ocupado";
    public const string PagoFallido = "Pago fallido";
    public const string PagoNoDisponible = "Pago no disponible";
}
