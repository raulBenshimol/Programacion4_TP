using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Availability.Application.Interfaces;
using Availability.Domain.Entities;

namespace Availability.Application.UseCases;

public class ObtenerResultadoSolicitudUseCase
{
    private readonly ISolicitudRepository _solicitudRepo;

    public ObtenerResultadoSolicitudUseCase(ISolicitudRepository solicitudRepo)
    {
        _solicitudRepo = solicitudRepo;
    }

    public async Task<SolicitudReserva?> Ejecutar(Guid solicitudId)
    {
        // Busca la solicitud en la base de datos y la devuelve.
        // Si no existe, devuelve null (el controlador se encargará de devolver un 404).
        return await _solicitudRepo.ObtenerPorId(solicitudId);
    }
}
