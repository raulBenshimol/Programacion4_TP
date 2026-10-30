using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Booking.Booking.Data;

// Solo la usa "dotnet ef" (migraciones, database update); la aplicación configura
// el DbContext por su cuenta. dotnet ef corre con la carpeta del proyecto (Booking/)
// como directorio actual, así que booking.db queda en Booking/.
public class BookingDbContextFactory : IDesignTimeDbContextFactory<BookingDbContext>
{
    public BookingDbContext CreateDbContext(string[] args)
    {
        // El appsettings del grupo está en Booking.API/ y ASP.NET no lo carga desde ahí,
        // por eso se lee esa ruta a mano. ConnectionStrings__BookingDb (variable de entorno)
        // también sirve para cambiarla.
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(Path.Combine("Booking.API", "appsettings.json"), optional: true)
            .AddEnvironmentVariables()
            .Build();

        // Valor por defecto como en Availability: anda aunque no encuentre el appsettings.
        var connectionString = configuration.GetConnectionString("BookingDb") ?? "Data Source=booking.db";

        return new BookingDbContext(new DbContextOptionsBuilder<BookingDbContext>()
            .UseSqlite(connectionString)
            .Options);
    }
}
