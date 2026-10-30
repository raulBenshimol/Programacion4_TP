using Availability.Application.Interfaces;
using Availability.Application.UseCases;
using Availability.Controllers.Presenters;
using Availability.Infrastructure;
using Availability.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Infraestructura (Base de datos, repositorios, broker)
builder.Services.AddInfrastructure(builder.Configuration);

// 2. Casos de Uso (Anillo 2)
builder.Services.AddScoped<SolicitarReservaInteractor>();
builder.Services.AddScoped<ConsultarDisponibilidadUseCase>();
builder.Services.AddScoped<ObtenerResultadoSolicitudUseCase>();

// 3. Presenter (Anillo 3)
builder.Services.AddScoped<ISolicitudOutputPort, SolicitudPresenter>();
builder.Services.AddScoped<SolicitudPresenter>(); // Lo registro también directo por si el controlador lo pide así

// 4. MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 5. (Dejamos esto comentado por ahora para hacer las migraciones en el próximo paso)
/*
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AvailabilityDbContext>();
    db.Database.EnsureCreated(); 
}
*/

app.UseRouting();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


// Esta línea es para que las herramientas de EF Core puedan leer la configuración
public partial class Program { }