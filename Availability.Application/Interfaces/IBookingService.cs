using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Availability.Application.Interfaces;
public interface IBookingService
{
    // Devuelve el estado de la reserva consultando a Booking
    Task<object> ObtenerEstadoReserva(Guid solicitudId);
}
