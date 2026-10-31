namespace AuthServer.Models
{
    // Datos del usuario que inició sesión con Gmail (salen del id_token de Google).
    public class UsuarioGoogle
    {
        public string Email { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
    }
}
