using Availability.Controllers.Controllers;
using AvailabilitySite.Models;
using Microsoft.AspNetCore.Mvc;

namespace AvailabilitySite.Controllers;

public class ReservasMvcController : Controller
{
    private readonly AvailabilityController _availabilityController;

    public ReservasMvcController(AvailabilityController availabilityController)
    {
        _availabilityController = availabilityController;
    }

    // Muestra los asientos disponibles del evento seleccionado.
    [HttpGet]
    public async Task<IActionResult> Index(int eventoId = 1)
    {
        var modelo = new ReservasViewModel
        {
            EventoId = eventoId
        };

        // Reutilizo el controlador lógico del anillo 3.
        modelo.Asientos = await _availabilityController.ConsultarAsientos(eventoId);

        if (modelo.Asientos.Count == 0)
        {
            modelo.Mensaje = "No hay asientos disponibles para este evento.";
        }

        return View(modelo);
    }
}