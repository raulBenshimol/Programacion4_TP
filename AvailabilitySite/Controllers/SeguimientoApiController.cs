
using Availability.Controllers.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace AvailabilitySite.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeguimientoApiController : ControllerBase
{
    private readonly SeguimientoController _seguimientoLogicController;
    private readonly ILogger<SeguimientoApiController> _logger;

    // Inyectamos el controlador lógico del Anillo 3 y el logger.
    public SeguimientoApiController(
        SeguimientoController seguimientoLogicController,
        ILogger<SeguimientoApiController> logger)
    {
        _seguimientoLogicController = seguimientoLogicController;
        _logger = logger;
    }

    // GET: api/SeguimientoApi/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerEstado(Guid id)
    {
        _logger.LogInformation(
            "[AUDITORIA] INICIO API - Consulta de seguimiento. SolicitudId: {SolicitudId}",
            id);

        try
        {
            var resultado = await _seguimientoLogicController.ObtenerEstado(id);

            // Identificamos cuando la solicitud no existe en Availability.
            var propiedadMensaje = resultado.GetType().GetProperty("Mensaje");

            if (propiedadMensaje?.GetValue(resultado) is string mensaje &&
                mensaje == "Solicitud no encontrada en Availability")
            {
                _logger.LogWarning(
                    "[AUDITORIA] FIN API - Solicitud no encontrada. SolicitudId: {SolicitudId}",
                    id);

                return NotFound(new
                {
                    Mensaje = "Solicitud no encontrada en Availability"
                });
            }

            _logger.LogInformation(
                "[AUDITORIA] FIN API - Consulta de seguimiento. SolicitudId: {SolicitudId}",
                id);

            return Ok(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "[AUDITORIA] ERROR API - Consulta de seguimiento. SolicitudId: {SolicitudId}",
                id);

            return StatusCode(500, new
            {
                Mensaje = "Error interno al consultar el estado de la solicitud."
            });
        }
    }
}
