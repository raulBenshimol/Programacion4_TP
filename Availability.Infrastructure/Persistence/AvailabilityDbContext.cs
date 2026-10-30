using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Availability.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Availability.Infrastructure.Persistence;

public class AvailabilityDbContext : DbContext
{
    public AvailabilityDbContext(DbContextOptions<AvailabilityDbContext> options) : base(options) { }

    public DbSet<Asiento> Asientos => Set<Asiento>();
    public DbSet<SolicitudReserva> Solicitudes => Set<SolicitudReserva>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Le decimos que busque las configuraciones (los archivos que crearemos abajo)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AvailabilityDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}