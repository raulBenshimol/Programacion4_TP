using Availability.Application.Interfaces;
using Availability.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Availability.Application.UseCases;

public class ConsultarDisponibilidadUseCase
{
    private readonly IAsientoRepository _asientoRepo;
    private readonly ILogger<ConsultarDisponibilidadUseCase> _logger;

    public ConsultarDisponibilidadUseCase(
        IAsientoRepository asientoRepo,
        ILogger<ConsultarDisponibilidadUseCase> logger)
    {
        _asientoRepo = asientoRepo;
        _logger = logger;
    }

    public async Task<List<Asiento>> Ejecutar(int eventoId)
    {
        // Auditoría: inicio de la consulta.
        _logger.LogInformation(
            "[AUDITORIA] INICIO - Consulta disponibilidad. EventoId: {EventoId}",
            eventoId);

        var asientos = await _asientoRepo.ObtenerDisponibles(eventoId);

        // Auditoría: fin de la consulta.
        _logger.LogInformation(
            "[AUDITORIA] FIN - Consulta disponibilidad. EventoId: {EventoId}, Cantidad: {Cantidad}",
            eventoId, asientos.Count);

        return asientos;
    }
}
