using Availability.Application.Interfaces;
using Availability.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Availability.Application.UseCases;

public class ConsultarDisponibilidadUseCase
{
    private readonly IAsientoRepository _asientoRepo;

    public ConsultarDisponibilidadUseCase(IAsientoRepository asientoRepo)
    {
        _asientoRepo = asientoRepo;
    }

    public async Task<List<Asiento>> Ejecutar(int eventoId)
    {
        return await _asientoRepo.ObtenerDisponibles(eventoId);
    }
}
