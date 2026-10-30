using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Availability.Application.Interfaces;
using Availability.Controllers.DTOs;


namespace Availability.Controllers.Presenters;

public class SolicitudPresenter : ISolicitudOutputPort
{
    public SolicitudResponseDto Response { get; private set; } = new();

    public Task PresentarResultado(Guid solicitudId, string estado, string mensaje)
    {
        Response = new SolicitudResponseDto
        {
            SolicitudId = solicitudId,
            Estado = estado,
            Mensaje = mensaje
        };

        return Task.CompletedTask;
    }
}