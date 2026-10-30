using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Availability.Application.Interfaces;
using Availability.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Availability.Infrastructure.Persistence.Repositories;

public class SolicitudRepository : ISolicitudRepository
{
    private readonly AvailabilityDbContext _context;

    public SolicitudRepository(AvailabilityDbContext context) => _context = context;

    public async Task Guardar(SolicitudReserva solicitud)
    {
        await _context.Solicitudes.AddAsync(solicitud);
        await _context.SaveChangesAsync();
    }

    public async Task<SolicitudReserva?> ObtenerPorId(Guid id)
    {
        return await _context.Solicitudes.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task Actualizar(SolicitudReserva solicitud)
    {
        _context.Solicitudes.Update(solicitud);
        await _context.SaveChangesAsync();
    }
}
