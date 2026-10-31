using System.Net.Http.Headers;

namespace AvailabilitySite.Auth
{
    // Intermediario del HttpClient de Booking, antes de cada pedido le agrega "Authorization: Bearer <token>"
    // Así el código que llama a Booking (SeguimientoController) no cambia.
    public class TokenHandler : DelegatingHandler
    {
        private readonly TokenClient _tokenClient;

        public TokenHandler(TokenClient tokenClient)
        {
            _tokenClient = tokenClient;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _tokenClient.ObtenerTokenAsync();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}