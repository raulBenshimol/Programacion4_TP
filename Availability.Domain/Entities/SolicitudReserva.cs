using Availability.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Availability.Domain.Entities
{
    public class SolicitudReserva
    {
        public Guid Id { get; set; }
        public int EventoId { get; set; }   
        public int AsientoId { get; set; }
        public int UsuarioId { get; set; }
        public EstadoSolicitud Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public MotivoRechazo? MotivoRechazo { get; set; }
    }
}
