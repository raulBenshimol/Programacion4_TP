using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Availability.Application.Interfaces;
using Availability.Domain.Entities;

namespace Availability.Infrastructure.Messaging;

public class AzureBroker : IMessageBroker
{
    public Task PublicarSolicitud(SolicitudReserva solicitud)
    {
        // Como todavía no conectamos un Broker real (RabbitMQ o Azure),
        // simulamos el envío imprimiendo un mensaje en la consola.
        Console.WriteLine("==================================================");
        Console.WriteLine($"[BROKER] ¡Mensaje publicado con éxito!");
        Console.WriteLine($"SolicitudId: {solicitud.Id}");
        Console.WriteLine($"AsientoId: {solicitud.AsientoId}");
        Console.WriteLine($"UsuarioId: {solicitud.UsuarioId}");
        Console.WriteLine("==================================================");

        return Task.CompletedTask;
    }
}
