using System.Security.Claims;
using AuthServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace AuthServer.Controllers
{
    // Endpoint de tokens al estilo OAuth2: POST /connect/token con los datos como formulario.
    [ApiController]
    [Route("connect/token")]
    public class TokenController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly ILogger<TokenController> _logger;

        public TokenController(TokenService tokenService, ILogger<TokenController> logger)
        {
            _tokenService = tokenService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post(
            [FromForm(Name = "grant_type")] string grantType,
            [FromForm(Name = "client_id")] string? clientId,
            [FromForm(Name = "client_secret")] string? clientSecret,
            [FromForm(Name = "id_token")] string? idToken)
        {
            if (grantType == "client_credentials")
            {
                if (!_tokenService.ValidarCliente(clientId ?? "", clientSecret ?? ""))
                {
                    _logger.LogWarning("Cliente rechazado: {ClientId}", clientId);
                    return Unauthorized();
                }

                // No se loguea el token (acuerdo de seguridad mínima).
                _logger.LogInformation("Token emitido para el microservicio {ClientId}", clientId);
                return Ok(_tokenService.CrearToken(clientId!, new[] { new Claim("client_id", clientId!) }));
            }

            // El usuario ya inició sesión con Gmail; el sitio nos manda el id_token de Google.
            if (grantType == "google")
            {
                var usuario = await _tokenService.ValidarTokenGoogleAsync(idToken ?? "");
                if (usuario == null)
                {
                    _logger.LogWarning("id_token de Google rechazado");
                    return Unauthorized();
                }

                _logger.LogInformation("Token emitido para el usuario de Google {Email}", usuario.Email);
                return Ok(_tokenService.CrearToken(usuario.Email, new[]
                {
                    new Claim(JwtRegisteredClaimNames.Name, usuario.Nombre),
                    new Claim(JwtRegisteredClaimNames.Email, usuario.Email)
                }));
            }
            return BadRequest(new { error = "unsupported_grant_type" });
        }
    }
}
