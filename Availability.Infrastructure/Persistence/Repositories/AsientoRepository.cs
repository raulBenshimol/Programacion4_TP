using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Availability.Application.Interfaces;
using Availability.Domain.Entities;
using Availability.Domain.Enum;
using Microsoft.EntityFrameworkCore;

namespace Availability.Infrastructure.Persistence.Repositories;

public class AsientoRepository : IAsientoRepository
{
    private readonly AvailabilityDbContext _context;

    public AsientoRepository(AvailabilityDbContext context) => _context = context;

    public async Task<List<Asiento>> ObtenerDisponibles(int eventoId)
    {
        // Buscamos los asientos del evento que estén en estado 'libre'
        return await _context.Asientos
            .Where(a => a.EventoId == eventoId && a.Estado == EstadoAsiento.libre)
            .ToListAsync();
    }

    public async Task<Asiento> ObtenerPorId(int asientoId)
    {
        // Nota: Como tu interfaz no tiene '?' (nullable), esto podría devolver null si no existe.
        // Lo dejamos así para respetar tu código, pero tenelo en cuenta.
        return await _context.Asientos.FirstOrDefaultAsync(a => a.Id == asientoId);
    }
}