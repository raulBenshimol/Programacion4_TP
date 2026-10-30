using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using Availability.Application.Interfaces;
using Availability.Domain.Entities;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;

namespace Availability.Infrastructure.Messaging;

public class AzureBroker : IMessageBroker
{
    private readonly ServiceBusSender _sender;
    private readonly string _queueName = "grupo8"; // El EntityPath de mi cadena del grupo 8

    public AzureBroker(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("AzureServiceBus");
        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentNullException(nameof(connectionString), "Falta la cadena de conexión de Azure Service Bus.");

        var client = new ServiceBusClient(connectionString);
        _sender = client.CreateSender(_queueName);
    }

    public async Task PublicarSolicitud(SolicitudReserva solicitud)
    {
        // Mensaje que se va enviar por el broker 
        var mensaje = new
        {
            SolicitudId = solicitud.Id,
            EventoId = solicitud.EventoId, 
            AsientoId = solicitud.AsientoId,
            UsuarioId = solicitud.UsuarioId
        };

        var json = JsonSerializer.Serialize(mensaje);
        var serviceBusMessage = new ServiceBusMessage(json);

        await _sender.SendMessageAsync(serviceBusMessage);
        Console.WriteLine($"[BROKER-AZURE] Mensaje enviado: {json}");
    }
}
