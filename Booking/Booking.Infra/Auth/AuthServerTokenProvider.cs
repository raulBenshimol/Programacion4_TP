//Bruno Scucimarri
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Booking.Booking.Infra.Auth
{
    // OAuth2 Client Credentials: Booking se identifica ante AuthServer como servicio.
    // Es singleton para reutilizar el token hasta que esté por vencer.
    public class AuthServerTokenProvider
    {
        // Margen para no mandar un token que venza mientras viaja la llamada.
        private static readonly TimeSpan MargenRenovacion = TimeSpan.FromMinutes(1);

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly SemaphoreSlim _renovacion = new(1, 1);

        private string? _token;
        private DateTime _renovarDesde;

        public AuthServerTokenProvider(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<string> ObtenerToken()
        {
            await _renovacion.WaitAsync();
            try
            {
                if (_token is null || DateTime.UtcNow >= _renovarDesde)
                {
                    await PedirToken();
                }

                return _token!;
            }
            finally
            {
                _renovacion.Release();
            }
        }

        private async Task PedirToken()
        {
            var formulario = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = _configuration["AuthServer:ClientId"]!,
                ["client_secret"] = _configuration["AuthServer:ClientSecret"]!
            });

            var url = new Uri(new Uri(_configuration["AuthServer:Url"]!), "connect/token");
            HttpResponseMessage respuesta = await _httpClientFactory.CreateClient().PostAsync(url, formulario);
            respuesta.EnsureSuccessStatusCode();

            TokenResponse token = await respuesta.Content.ReadFromJsonAsync<TokenResponse>()
                ?? throw new InvalidOperationException("AuthServer devolvió una respuesta vacía.");

            _token = token.AccessToken;
            _renovarDesde = DateTime.UtcNow.AddSeconds(token.ExpiresIn) - MargenRenovacion;
        }

        private sealed class TokenResponse
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; init; } = string.Empty;

            [JsonPropertyName("expires_in")]
            public int ExpiresIn { get; init; }
        }
    }
}
