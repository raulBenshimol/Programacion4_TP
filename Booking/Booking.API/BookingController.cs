using Booking.Booking.Entities.Booking.API.DTOs;
using Booking.Booking.Entities.Booking.Entities.Enums;
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
        public BookingController(ILogger<BookingController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{id}")]
        public ActionResult<BookingResponseDto> GetReservationStatusById(Guid id)
        {
            _logger.LogInformation("Request recieved");
            
            return Ok(new BookingResponseDto {
                id = id,
                status = ReservationStatusEnum.Rechazada,
                RejectionMotive = "El motivo es que todavia no esta la DB implementada"
               }
            );
        } 
    }
}
