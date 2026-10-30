using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Availability.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Availability.Infrastructure.Persistence.Configurations;

public class SolicitudReservaConfiguration : IEntityTypeConfiguration<SolicitudReserva>
{
    public void Configure(EntityTypeBuilder<SolicitudReserva> builder)
    {
        builder.ToTable("Solicitudes");
        builder.HasKey(s => s.Id);

        // Guardamos los Enums como texto
        builder.Property(s => s.Estado).HasConversion<string>().HasMaxLength(20);
        builder.Property(s => s.MotivoRechazo).HasConversion<string>().HasMaxLength(20);
    }
}