using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Booking.Booking.Data;

// Solo para dotnet ef (migraciones); la aplicación configura el DbContext por su cuenta.
public class BookingDbContextFactory : IDesignTimeDbContextFactory<BookingDbContext>
{
    public BookingDbContext CreateDbContext(string[] args)
    {
        // El appsettings está en Booking.API/ y ASP.NET no lo carga desde ahí: se lee a mano.
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(Path.Combine("Booking.API", "appsettings.json"), optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("BookingDb") ?? "Data Source=booking.db";

        return new BookingDbContext(new DbContextOptionsBuilder<BookingDbContext>()
            .UseSqlite(connectionString)
            .Options);
    }
}
