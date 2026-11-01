using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Availability.Application.UseCases;
using Availability.Application.Interfaces;

namespace Availability.Controllers.Controllers;

public class SeguimientoController
{
    private readonly ObtenerResultadoSolicitudUseCase _obtenerResultado;
    private readonly IBookingService _bookingService;

    public SeguimientoController(
        ObtenerResultadoSolicitudUseCase obtenerResultado,
        IBookingService bookingService)
    {
        _obtenerResultado = obtenerResultado;
        _bookingService = bookingService;
    }

    public async Task<object> ObtenerEstado(Guid solicitudId)
    {
        // 1. Buscamos la solicitud en nuestra base local
        var solicitud = await _obtenerResultado.Ejecutar(solicitudId);

        if (solicitud == null)
            return new { Mensaje = "Solicitud no encontrada en Availability" };

        // 2. Consultamos a Booking a traves de la interfaz sin romper los dominios anemicas y la arqitectura de afuera hacia adentro.
        try
        {
            var bookingResponse = await _bookingService.ObtenerEstadoReserva(solicitudId);

            return new
            {
                 SolicitudId = solicitudId,
                 EstadoLocal = solicitud.Estado.ToString(),
                 Booking = bookingResponse
            };
        }
        catch (Exception ex)
        {
            // Si Booking está caído, devolvemos el estado local con una advertencia
            return new
            {
                SolicitudId = solicitudId,
                EstadoLocal = solicitud.Estado.ToString(),
                Booking = $"Error al conectar con Booking: {ex.Message}"
            };
        }
    }
}