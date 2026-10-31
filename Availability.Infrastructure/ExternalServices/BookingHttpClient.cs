using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;
using Availability.Application.Interfaces;

namespace Availability.Infrastructure.ExternalServices;

public class BookingHttpClient : IBookingService
{
    private readonly HttpClient _httpClient;

    public BookingHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<object> ObtenerEstadoReserva(Guid solicitudId)
    {
        // Correccion de infra aca es donde debe ir httpClient
        var response = await _httpClient.GetAsync($"Booking/{solicitudId}");

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<object>() ?? "Sin respuesta";
        }

        return "No encontrada en Booking (posiblemente aún en proceso)";
    }
}
