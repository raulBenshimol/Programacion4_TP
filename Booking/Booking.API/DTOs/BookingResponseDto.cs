using Booking.Booking.Entities.Booking.Entities.Enums;

namespace Booking.Booking.Entities.Booking.API.DTOs
{
    public class BookingResponseDto
    {
        public Guid id {  get; set; }
        public ReservationStatusEnum status { get; set; } 
        public string? RejectionMotive { get; set; }
    }
}
 