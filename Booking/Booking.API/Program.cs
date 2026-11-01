using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Booking.Auth;
using SistemaReservas.Contracts.Payment.V1;
var builder = WebApplication.CreateBuilder(args);
// El appsettings.json de Booking está dentro de la carpeta Booking.API, no en la raíz del proyecto.
builder.Configuration.AddJsonFile("Booking.API/appsettings.json", optional: false);

// Add services to the container.

builder.Services.AddControllers();
// Validación del JWT emitido por el AuthServer: se verifica la firma con su clave PÚBLICA (RS256).
// Booking puede comprobar tokens pero no fabricarlos (no tiene la clave privada).
var rsa = RSA.Create();
rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(builder.Configuration["Jwt:ClavePublica"]!), out _);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new RsaSecurityKey(rsa)
        };
    });
builder.Services.AddAuthorization();
// Booking -> Payment (gRPC): Booking pide su token al AuthServer (client credentials)
// y se agrega solo en la Metadata ("Authorization: Bearer ...") de cada llamada a Payment.
builder.Services.AddHttpClient();
builder.Services.AddSingleton<TokenClient>();
builder.Services.AddGrpcClient<PaymentService.PaymentServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration["Payment:Url"]!);
})
    .AddCallCredentials(async (context, metadata, serviceProvider) =>
    {
        var token = await serviceProvider.GetRequiredService<TokenClient>().ObtenerTokenAsync();
        metadata.Add("Authorization", $"Bearer {token}");
    });

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // primero verifica quién sos (lee y valida el token)

app.UseAuthorization();

app.MapControllers();

app.Run();
