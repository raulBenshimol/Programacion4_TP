using System.Security.Claims;
using AuthServer.Services;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult Post(
            [FromForm(Name = "grant_type")] string grantType,
            [FromForm(Name = "client_id")] string? clientId,
            [FromForm(Name = "client_secret")] string? clientSecret)
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

            return BadRequest(new { error = "unsupported_grant_type" });
        }
    }
}
