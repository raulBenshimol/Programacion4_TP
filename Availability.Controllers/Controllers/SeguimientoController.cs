using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Availability.Application.UseCases;

namespace Availability.Controllers.Controllers;

public class SeguimientoController
{
    private readonly ObtenerResultadoSolicitudUseCase _obtenerResultado;

    public SeguimientoController(ObtenerResultadoSolicitudUseCase obtenerResultado)
    {
        _obtenerResultado = obtenerResultado;
    }

    public async Task<object> ObtenerEstado(Guid solicitudId)
    {
        var solicitud = await _obtenerResultado.Ejecutar(solicitudId);

        if (solicitud == null)
            return new { Mensaje = "Solicitud no encontrada" };

        return new
        {
            Id = solicitud.Id,
            Estado = solicitud.Estado.ToString(),
            MotivoRechazo = solicitud.MotivoRechazo?.ToString()
        };
    }
}
