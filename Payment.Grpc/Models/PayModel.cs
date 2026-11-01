namespace Payment.Grpc.Models
{
    public class PayModel
    {
        public int Id { get; set; }
        public Guid SolicitudId { get; set; }
        public int ReservaId { get; set; }
        public string UsuarioId { get; set; } = string.Empty;
        public bool Aprobado { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaTransaccion { get; set; }

    }
}