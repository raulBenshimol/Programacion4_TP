namespace Booking.Booking.Business.Interfaces
{
    public interface IValidator<T>
    {
        // Devuelve los errores encontrados; si no hay ninguno, resultado trae el objeto leído.
        IReadOnlyList<string> Validate(string contenido, out T? resultado);
    }
}
