using Serilog;
using Microsoft.OpenApi.Models;
using Availability.Application.Interfaces;
using Availability.Application.UseCases;
using Availability.Controllers.Presenters;
using Availability.Infrastructure;
using Availability.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Configuracion de logs: consola y de archivo
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
    .MinimumLevel.Information()

    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)

    .WriteTo.Console()
    .WriteTo.File(
       "logs/availability-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7);
});

// 1. Infraestructura (Base de datos, repositorios, broker)
builder.Services.AddInfrastructure(builder.Configuration);

// 2. Casos de Uso (Anillo 2)
builder.Services.AddScoped<SolicitarReservaInteractor>();
builder.Services.AddScoped<ConsultarDisponibilidadUseCase>();
builder.Services.AddScoped<ObtenerResultadoSolicitudUseCase>();


// 3. Presenter (Anillo 3) - CAMBIO: Registramos una sola instancia para evitar el GUID en cero
builder.Services.AddScoped<SolicitudPresenter>();
builder.Services.AddScoped<ISolicitudOutputPort>(sp => sp.GetRequiredService<SolicitudPresenter>());


// Registramos los controladores lógicos del Anillo 3 para que puedan ser utilizados mediante inyección de dependencias.
// Controladores Lógicos del Anillo 3.1 
builder.Services.AddScoped<Availability.Controllers.Controllers.AvailabilityController>();
builder.Services.AddScoped<Availability.Controllers.Controllers.SeguimientoController>();

// CAMBIO: Cliente HTTP para llamar a Booking (el de tu compañero)
builder.Services.AddHttpClient<IBookingService, Availability.Infrastructure.ExternalServices.BookingHttpClient>(
    client =>
    {
        client.BaseAddress = new Uri("https://localhost:7109/");
    });
// 4. MVC
builder.Services.AddControllersWithViews();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Habilitar Swagger en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 5. (Dejamos esto comentado por ahora para hacer las migraciones en el próximo paso)
/*
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AvailabilityDbContext>();
    db.Database.EnsureCreated(); 
}
*/


// Cargar asientos de prueba únicamente en desarrollo.
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    var db = scope.ServiceProvider
        .GetRequiredService<AvailabilityDbContext>();

    await AsientosSeed.InicializarAsync(db);
}

app.UseStaticFiles();

app.UseRouting();
app.MapControllerRoute(
    name: "default",
    //pattern: "{controller=Home}/{action=Index}/{id?}");
    pattern: "{controller=ReservasMvc}/{action=Index}/{id?}");

app.Run();


// Esta línea es para que las herramientas de EF Core puedan leer la configuración
public partial class Program { }