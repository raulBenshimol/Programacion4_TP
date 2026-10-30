using Availability.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Availability.Application.Interfaces
{
    public interface IMessageBroker
    {
        Task PublicarSolicitud(SolicitudReserva solicitud);

    }
}
