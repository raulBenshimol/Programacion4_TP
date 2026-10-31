using Availability.Application.Interfaces;
using Availability.Domain.Entities;
using Availability.Domain.Enum;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Availability.Application.UseCases;

public class SolicitarReservaInteractor
{
    private readonly ISolicitudRepository _solicitudRepo;
    private readonly IMessageBroker _broker;
    private readonly ISolicitudOutputPort _outputPort;

    public SolicitarReservaInteractor(
        ISolicitudRepository solicitudRepo,
        IMessageBroker broker,
        ISolicitudOutputPort outputPort)
    {
        _solicitudRepo = solicitudRepo;
        _broker = broker;
        _outputPort = outputPort;
    }

    public async Task Ejecutar(int asientoId, int usuarioId, int eventoId)
    {
        // 1. Crear la entidad
        var solicitud = new SolicitudReserva
        {
            Id = Guid.NewGuid(),
            EventoId = eventoId,
            AsientoId = asientoId,
            UsuarioId = usuarioId,
            Estado = EstadoSolicitud.PendientePago,
            FechaCreacion = DateTime.UtcNow
        };

        // 2. Guardar en Base de Datos (a través de la interfaz)
        await _solicitudRepo.Guardar(solicitud);

        // 3. Publicar al broker
        await _broker.PublicarSolicitud(solicitud);

        // 4. Devolver el resultado a través del Presenter (Output Port)
        await _outputPort.PresentarResultado(solicitud.Id, "Pendiente", "Solicitud recibida y en proceso.");
    }
}
