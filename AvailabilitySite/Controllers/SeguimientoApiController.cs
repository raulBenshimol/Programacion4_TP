using Availability.Controllers.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace AvailabilitySite.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeguimientoApiController : ControllerBase
{
    private readonly SeguimientoController _seguimientoLogicController;

    public SeguimientoApiController(SeguimientoController seguimientoLogicController)
    {
        _seguimientoLogicController = seguimientoLogicController;
    }

    // GET: api/seguimiento/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerEstado(Guid id)
    {
        var resultado = await _seguimientoLogicController.ObtenerEstado(id);
        return Ok(resultado);
    }
}