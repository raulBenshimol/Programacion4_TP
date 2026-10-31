
using Availability.Controllers.Controllers;
using Availability.Controllers.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AvailabilitySite.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservasApiController : ControllerBase
{
    private readonly AvailabilityController _availabilityLogicController;
    private readonly ILogger<ReservasApiController> _logger;

    // Inyectamos el controlador lógico del Anillo 3 y el logger.
    public ReservasApiController(
        AvailabilityController availabilityLogicController,
        ILogger<ReservasApiController> logger)
    {
        _availabilityLogicController = availabilityLogicController;
        _logger = logger;
    }

    // GET: api/ReservasApi/asientos?eventoId=1
    [HttpGet("asientos")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ConsultarAsientos([FromQuery] int eventoId)
    {
        _logger.LogInformation(
            "[AUDITORIA] INICIO API - Consulta de asientos. EventoId: {EventoId}",
            eventoId);

        // Validamos que el identificador sea positivo.
        if (eventoId <= 0)
        {
            _logger.LogWarning(
                "[AUDITORIA] FIN API - EventoId inválido: {EventoId}",
                eventoId);

            return BadRequest(new
            {
                Mensaje = "El identificador del evento debe ser mayor que cero."
            });
        }

        try
        {
            var asientos = await _availabilityLogicController
                .ConsultarAsientos(eventoId);

            _logger.LogInformation(
                "[AUDITORIA] FIN API - Consulta de asientos. EventoId: {EventoId}, Cantidad: {Cantidad}",
                eventoId, asientos.Count);

            return Ok(asientos);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "[AUDITORIA] ERROR API - Consulta de asientos. EventoId: {EventoId}",
                eventoId);

            return StatusCode(500, new
            {
                Mensaje = "Error interno al consultar los asientos."
            });
        }
    }

    // POST: api/ReservasApi/solicitar
    [HttpPost("solicitar")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SolicitarReserva(
        [FromBody] SolicitudRequestDto request)
    {
        _logger.LogInformation(
            "[AUDITORIA] INICIO API - Solicitud de reserva. EventoId: {EventoId}, AsientoId: {AsientoId}",
            request.EventoId, request.AsientoId);

        // Validamos los identificadores recibidos.
        if (request.EventoId <= 0 ||
            request.AsientoId <= 0 ||
            request.UsuarioId <= 0)
        {
            _logger.LogWarning(
                "[AUDITORIA] FIN API - Solicitud con identificadores inválidos.");

            return BadRequest(new
            {
                Mensaje = "Los identificadores del evento, asiento y usuario deben ser mayores que cero."
            });
        }

        try
        {
            var response = await _availabilityLogicController
                .SolicitarReserva(request);

            _logger.LogInformation(
                "[AUDITORIA] FIN API - Solicitud de reserva. SolicitudId: {SolicitudId}",
                response.SolicitudId);

            // La solicitud fue recibida; Booking debe procesarla.
            return Accepted(value: response);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "[AUDITORIA] ERROR API - Solicitud de reserva. EventoId: {EventoId}, AsientoId: {AsientoId}",
                request.EventoId, request.AsientoId);

            return StatusCode(500, new
            {
                Mensaje = "Error interno al procesar la solicitud."
            });
        }
    }
}
