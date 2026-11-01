//Bruno Scucimarri
using Booking.Booking.Entities.Booking.API.DTOs;

namespace Booking.Booking.Entities.Booking.Business.Interfaces
{
    
    public interface IBookingService
    {
        Task<BookingResponseDto> ProcessReservation(Guid solicitudId, int eventoId, int seatId, int userId);
        Task<BookingResponseDto?> GetReservationStatusById(Guid reservationId);
    } 
}
