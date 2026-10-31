using Availability.Controllers.DTOs;

namespace AvailabilitySite.Models
{
    public class ReservasViewModel
    {
        // Identificador del evento seleccionado
        public int EventoId { get; set; } = 1;

        // Lista de asientos obtenidos desde Availability
        public List<AsientoDto> Asientos { get; set; } = new();

        // Mensaje informativo para mostrar en la vista
        public string Mensaje { get; set; } = string.Empty;

        // Identificador para consultar el seguimiento de una solicitud
        public Guid? SolicitudId { get; set; }
    }
}
