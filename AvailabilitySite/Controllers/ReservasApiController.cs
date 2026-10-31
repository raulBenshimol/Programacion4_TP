using Availability.Application.UseCases;
using Availability.Controllers.Controllers;
using Availability.Controllers.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace AvailabilitySite.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservasApiController : ControllerBase
{
    private readonly AvailabilityController _availabilityLogicController;

    // Inyectamos el controlador LÓGICO del Anillo 3
    public ReservasApiController(AvailabilityController availabilityLogicController)
    {
        _availabilityLogicController = availabilityLogicController;
    }

    // GET: api/reservas/asientos?eventoId=1
    [HttpGet("asientos")]
    public async Task<IActionResult> ConsultarAsientos([FromQuery] int eventoId)
    {
        var asientos = await _availabilityLogicController.ConsultarAsientos(eventoId);
        return Ok(asientos);
    }

    // POST: api/reservas/solicitar
    [Authorize] // solo un usuario logueado con Google puede solicitar una reserva
    [HttpPost("solicitar")]
    public async Task<IActionResult> SolicitarReserva([FromBody] SolicitudRequestDto request)
    {
        var response = await _availabilityLogicController.SolicitarReserva(request);
        return Ok(response);
    }
}