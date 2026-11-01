using Availability.Application.Interfaces;
using Availability.Domain.Entities;
using Availability.Domain.Enum;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Availability.Application.UseCases;

public class SolicitarReservaInteractor
{
    private readonly ISolicitudRepository _solicitudRepo;
    private readonly IMessageBroker _broker;
    private readonly ISolicitudOutputPort _outputPort;
    private readonly ILogger<SolicitarReservaInteractor> _logger;

    public SolicitarReservaInteractor(
        ISolicitudRepository solicitudRepo,
        IMessageBroker broker,
        ISolicitudOutputPort outputPort,
        ILogger<SolicitarReservaInteractor> logger)
    {
        _solicitudRepo = solicitudRepo;
        _broker = broker;
        _outputPort = outputPort;
        _logger = logger;
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


        // Auditoria: Inicio de solicitud
        _logger.LogInformation(
            "[AUDITORIA] INICIO - Solicitud de reserva. SolicitudId: {SolicitudId}, EventoId: {EventoId}, AsientoId: {AsientoId}",
           solicitud.Id, eventoId, asientoId);

        try
        {
            // 2. Guardar Solicitud
            await _solicitudRepo.Guardar(solicitud);

            _logger.LogInformation(
                "[AUDITORIA] Solicitud guardada. SolicitudId: {SolicitudId}",
                solicitud.Id);

            // 3. Publicar al broker
            await _broker.PublicarSolicitud(solicitud);

            _logger.LogInformation(
               "[AUDITORIA] Solicitud publicada en broker. SolicitudId: {SolicitudId}",
               solicitud.Id);


            // 4. Devolver el resultado a través del Presenter (Output Port)
            await _outputPort.PresentarResultado(solicitud.Id, "Pendiente", "Solicitud recibida y en proceso.");

            _logger.LogInformation(
              "[AUDITORIA] FIN - Solicitud de reserva. SolicitudId: {SolicitudId}",
              solicitud.Id);

        }
        catch (Exception ex) {

            // Auditoria: registrar el error y conservar la excepcion.
            _logger.LogError(
                ex,
              "[AUDITORIA] ERROR - Solicitud de reserva. SolicitudId: {SolicitudId}",
              solicitud.Id);

            throw;
        }
    }
}





        

