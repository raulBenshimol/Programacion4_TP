using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Availability.Domain.Entities;
using Availability.Domain.Enum;


namespace Availability.Infrastructure.Persistence;

public static class AsientosSeed
{
    public static async Task InicializarAsync(AvailabilityDbContext context)
    {
        // Asientos ficticios para probar la interfaz MVC.
        for (int eventoId = 1; eventoId <= 3; eventoId++)
        {
            for (int numero = 1; numero <= 8; numero++)
            {
                string codigo = $"A{numero}";

                // Evita duplicar asientos al reiniciar la aplicación.
                bool existe = await context.Asientos.AnyAsync(a =>
                    a.EventoId == eventoId && a.Codigo == codigo);

                if (existe)
                    continue;

                var asiento = new Asiento
                {
                    EventoId = eventoId,
                    Codigo = codigo,
                    Estado = numero <= 5
                        ? EstadoAsiento.libre
                        : numero <= 7
                            ? EstadoAsiento.reservado
                            : EstadoAsiento.ocupado
                };

                context.Asientos.Add(asiento);
            }
        }

        // Guarda únicamente los asientos nuevos.
        await context.SaveChangesAsync();
    }
}

