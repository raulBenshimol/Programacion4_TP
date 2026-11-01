using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Availability.Application.Interfaces;
using Availability.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Availability.Application.UseCases;

public class ObtenerResultadoSolicitudUseCase
{
    private readonly ISolicitudRepository _solicitudRepo;
    private readonly ILogger<ObtenerResultadoSolicitudUseCase> _logger;

    public ObtenerResultadoSolicitudUseCase(ISolicitudRepository solicitudRepo,
        ILogger<ObtenerResultadoSolicitudUseCase> logger)
    {
        _solicitudRepo = solicitudRepo;
        _logger = logger;
    }

    public async Task<SolicitudReserva?> Ejecutar(Guid solicitudId)
    {
        // Auditoria: inicio de consulta.
        _logger.LogInformation(
            "[AUDITORIA] INICIO - Consulta de solicitud. SolicitudId: {SolicitudId}",
            solicitudId);

        var solicitud = await _solicitudRepo.ObtenerPorId(solicitudId);

        // Auditoria: finalizacion de consulta
        _logger.LogInformation(
            "[AUDITORIA] FIN - Consulta de solicitud. SolicitudId: {SolicitudId}, Encontrado: {Encontrada}",
            solicitudId, solicitud != null);

        // Devuelve la solicitud encontrada o null si no existe.
        return solicitud;
    }
}
