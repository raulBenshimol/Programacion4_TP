using System.Security.Claims;
using System.Security.Cryptography;
using AuthServer.Models;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace AuthServer.Services
{
    // Valida a los clientes y fabrica el JWT firmado con la clave PRIVADA del AuthServer (RS256).
    public class TokenService
    {
        private readonly IConfiguration _configuration;
        private readonly RsaSecurityKey _clavePrivada;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;

            // La clave privada está en User Secrets: solo el AuthServer la tiene.
            var claveBase64 = configuration["Jwt:ClavePrivada"]
                ?? throw new InvalidOperationException("Falta Jwt:ClavePrivada en User Secrets");
            var rsa = RSA.Create();
            rsa.ImportRSAPrivateKey(Convert.FromBase64String(claveBase64), out _);
            _clavePrivada = new RsaSecurityKey(rsa);
        }

        // OAuth2 Client Credentials: el microservicio se identifica con su client_id y client_secret.
        public bool ValidarCliente(string clientId, string clientSecret)
        {
            var secretoEsperado = _configuration[$"Clientes:{clientId}"];
            return secretoEsperado != null && secretoEsperado == clientSecret;
        }

        // Claims vistos en clase: iss, sub, aud, exp, iat.
        public TokenResponse CrearToken(string subject, IEnumerable<Claim> claimsAdicionales)
        {
            var jwt = _configuration.GetSection("Jwt");
            var minutos = jwt.GetValue<int>("MinutosExpiracion");

            var claims = new List<Claim> { new(JwtRegisteredClaimNames.Sub, subject) };
            claims.AddRange(claimsAdicionales);

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = jwt["Issuer"],
                Audience = jwt["Audience"],
                Subject = new ClaimsIdentity(claims),
                IssuedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(minutos),
                SigningCredentials = new SigningCredentials(_clavePrivada, SecurityAlgorithms.RsaSha256)
            };

            return new TokenResponse
            {
                AccessToken = new JsonWebTokenHandler().CreateToken(descriptor),
                ExpiresIn = minutos * 60
            };
        }
    }
}
