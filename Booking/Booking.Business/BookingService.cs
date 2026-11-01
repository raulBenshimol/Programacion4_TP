//Bruno Scucimarri
using Booking.Booking.Business.Interfaces;
using Booking.Booking.Data;
using Booking.Booking.Entities;
using Booking.Booking.Entities.Booking.API.DTOs;
using Booking.Booking.Entities.Booking.Business.Interfaces;
using Booking.Booking.Entities.Booking.Entities.Enums;
using Booking.Booking.Entities.Excepciones;
using Booking.Booking.Entities.Interfaces;

namespace Booking.Booking.Business
{
    public class BookingService : IBookingService
    {
        private readonly IReservaRepository _reservaRepository;
        private readonly IPaymentClient _paymentClient;
        private readonly IConfiguration _configuration;
        public BookingService(IReservaRepository reservaRepository, IPaymentClient paymentClient, IConfiguration configuration) {
            _reservaRepository = reservaRepository;
            _paymentClient = paymentClient;
            _configuration = configuration;
        }
        public async Task<BookingResponseDto?> GetReservationStatusBySolicitudId(Guid solicitudId)
        {

            Reserva? reserva = await this._reservaRepository.ObtenerPorSolicitudId(solicitudId);

            if (reserva is null)
            {
                return null;
            }

            return MapearRespuesta(reserva);
        }

        public async Task<BookingResponseDto> ProcessReservation(Guid solicitudId, int eventoId, int seatId, int userId)
        {
            // Service Bus puede entregar el mismo mensaje dos veces: no se crea otra reserva.
            Reserva reserva = await _reservaRepository.ObtenerPorSolicitudId(solicitudId)
                ?? await RetenerAsiento(solicitudId, eventoId, seatId, userId);

            if (reserva.Estado != ReservationStatusEnum.Retenida)
            {
                return MapearRespuesta(reserva);
            }

            // Una entrega repetida también pasa por acá si el intento anterior no terminó de cobrar:
            // Payment no cobra dos veces la misma reserva y devuelve el resultado original.
            ResultadoPago resultado = await _paymentClient.ProcesarPago(reserva.SolicitudId, reserva.Id, reserva.Monto);

            reserva.Estado = resultado == ResultadoPago.Aprobado
                ? ReservationStatusEnum.Confirmada
                : ReservationStatusEnum.Rechazada;
            reserva.MotivoRechazo = resultado switch
            {
                ResultadoPago.Rechazado => MotivosRechazo.PagoFallido,
                ResultadoPago.NoDisponible => MotivosRechazo.PagoNoDisponible,
                _ => null
            };

            try
            {
                await _reservaRepository.Actualizar(reserva);
            }
            catch (ConcurrenciaReservaException)
            {
                // Otra réplica o el job de vencimiento la modificó primero: vale lo que quedó guardado.
                Reserva actual = await _reservaRepository.ObtenerPorId(reserva.Id)
                    ?? throw new InvalidOperationException($"No se encontró la reserva {reserva.Id}.");
                return MapearRespuesta(actual);
            }

            return MapearRespuesta(reserva);
        }

        private async Task<Reserva> RetenerAsiento(Guid solicitudId, int eventoId, int seatId, int userId)
        {
            DateTime ahora = DateTime.UtcNow;
            var reserva = new Reserva
            {
                Id = Guid.NewGuid(),
                SolicitudId = solicitudId,
                EventoId = eventoId,
                AsientoId = seatId.ToString(),
                UsuarioId = userId.ToString(),
                Estado = ReservationStatusEnum.Retenida,
                Monto = _configuration.GetValue<decimal>("Reservas:Monto"),
                FechaCreacion = ahora,
                FechaVencimiento = ahora.AddMinutes(_configuration.GetValue<int>("Reservas:MinutosRetencion"))
            };

            try
            {
                try
                {
                    // La decide el índice único de la base, así solo una réplica gana el asiento.
                    await _reservaRepository.Guardar(reserva);
                }
                catch (AsientoOcupadoException)
                {
                    reserva.Estado = ReservationStatusEnum.Rechazada;
                    reserva.MotivoRechazo = MotivosRechazo.AsientoOcupado;
                    await _reservaRepository.Guardar(reserva);
                }
            }
            catch (SolicitudDuplicadaException)
            {
                // Otra réplica procesó la misma solicitud entre la consulta inicial y el guardado.
                return await _reservaRepository.ObtenerPorSolicitudId(solicitudId)
                    ?? throw new InvalidOperationException($"No se encontró la reserva de la solicitud {solicitudId}.");
            }

            return reserva;
        }

        private static BookingResponseDto MapearRespuesta(Reserva reserva) => new()
        {
            id = reserva.Id,
            status = reserva.Estado,
            RejectionMotive = reserva.MotivoRechazo
        };
    }
}
