using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Availability.Application.UseCases;

namespace Availability.Controllers.Controllers;

public class SeguimientoController
{
    private readonly ObtenerResultadoSolicitudUseCase _obtenerResultado;
    private readonly IHttpClientFactory _httpClientFactory;

    public SeguimientoController(
        ObtenerResultadoSolicitudUseCase obtenerResultado,
        IHttpClientFactory httpClientFactory)
    {
        _obtenerResultado = obtenerResultado;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<object> ObtenerEstado(Guid solicitudId)
    {
        // 1. Buscamos la solicitud en nuestra base local
        var solicitud = await _obtenerResultado.Ejecutar(solicitudId);

        if (solicitud == null)
            return new { Mensaje = "Solicitud no encontrada en Availability" };

        // 2. Consultamos a Booking el estado real de la reserva
        try
        {
            var client = _httpClientFactory.CreateClient("BookingClient");

            // Llamamos al endpoint de tu compañero: GET /Booking/{solicitudId}
            var response = await client.GetAsync($"Booking/{solicitudId}");

            if (response.IsSuccessStatusCode)
            {
                // Si Booking responde OK, devolvemos su respuesta
                var bookingResponse = await response.Content.ReadFromJsonAsync<object>();
                return new
                {
                    SolicitudId = solicitudId,
                    EstadoLocal = solicitud.Estado.ToString(),
                    Booking = bookingResponse
                };
            }
            else
            {
                // Si Booking no la encontró (404), devolvemos el estado local
                return new
                {
                    SolicitudId = solicitudId,
                    EstadoLocal = solicitud.Estado.ToString(),
                    Booking = "No encontrada en Booking (posiblemente aún en proceso)"
                };
            }
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