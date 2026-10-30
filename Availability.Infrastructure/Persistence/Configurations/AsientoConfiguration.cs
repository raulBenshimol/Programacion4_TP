using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Availability.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Availability.Infrastructure.Persistence.Configurations;

public class AsientoConfiguration : IEntityTypeConfiguration<Asiento>
{
    public void Configure(EntityTypeBuilder<Asiento> builder)
    {
        builder.ToTable("Asientos");
        builder.HasKey(a => a.Id);

        // El código del asiento es un texto obligatorio de máximo 20 caracteres
        builder.Property(a => a.Codigo).IsRequired().HasMaxLength(20);

        // Guardamos el Enum como texto (string) en la base de datos para que se lea fácil
        builder.Property(a => a.Estado).HasConversion<string>().HasMaxLength(20);
    }
}