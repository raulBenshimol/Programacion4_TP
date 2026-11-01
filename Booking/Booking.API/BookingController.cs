//Bruno Scucimarri
using Booking.Booking.Entities.Booking.API.DTOs;
using Booking.Booking.Entities.Booking.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Booking.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly ILogger<BookingController> _logger;
        private readonly IBookingService _bookingService;
        public BookingController(ILogger<BookingController> logger, IBookingService bookingService)
        {
            _logger = logger;
            _bookingService = bookingService;
        }

        [HttpGet("{solicitudId}")]
        public async Task<ActionResult<BookingResponseDto>> GetReservationStatusBySolicitudId(Guid solicitudId)
        {
            _logger.LogInformation("Peticion recibida en Booking Controller");

            try
            {
                BookingResponseDto? respuesta = await _bookingService.GetReservationStatusBySolicitudId(solicitudId);

                if (respuesta is null)
                {
                    _logger.LogError("No se encontro la reserva solicitada");
                    return NotFound();
                }

                _logger.LogInformation("Reserva encontrada exitosamente");
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener la reserva de la solicitud {SolicitudId}", solicitudId);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
