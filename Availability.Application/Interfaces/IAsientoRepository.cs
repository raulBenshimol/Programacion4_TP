using Availability.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Availability.Application.Interfaces
{
    public interface IAsientoRepository
    {
        Task<List<Asiento>> ObtenerDisponibles(int eventoId);
            Task<Asiento> ObtenerPorId(int asientoId);
    }
}
