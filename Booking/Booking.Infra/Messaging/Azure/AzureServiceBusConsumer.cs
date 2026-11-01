//Bruno Scucimarri
using Azure.Messaging.ServiceBus;
using Booking.Booking.Business.Interfaces;
using Booking.Booking.Entities.Booking.Business.Interfaces;

namespace Booking.Booking.Infra.Messaging.Azure
{
    // BackgroundService ya aporta StartAsync y StopAsync, que son los que pide IMessageConsumer.
    // ServiceBusProcessor recibe los mensajes por push: no hay polling de nuestro lado.
    public class AzureServiceBusConsumer : BackgroundService, IMessageConsumer
    {
        private readonly ServiceBusClient _client;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IValidator<SolicitudReservaMensaje> _validator;
        private readonly ILogger<AzureServiceBusConsumer> _logger;
        private readonly string _topic;
        private readonly string _suscripcion;

        public AzureServiceBusConsumer(
            ServiceBusClient client,
            IServiceScopeFactory scopeFactory,
            IValidator<SolicitudReservaMensaje> validator,
            IConfiguration configuration,
            ILogger<AzureServiceBusConsumer> logger)
        {
            _client = client;
            _scopeFactory = scopeFactory;
            _validator = validator;
            _logger = logger;
            _topic = configuration["AzureServiceBus:TopicSolicitudes"] is { Length: > 0 } topic
                ? topic
                : throw new InvalidOperationException("Falta configurar AzureServiceBus:TopicSolicitudes.");
            _suscripcion = configuration["AzureServiceBus:SuscripcionSolicitudes"] is { Length: > 0 } suscripcion
                ? suscripcion
                : throw new InvalidOperationException("Falta configurar AzureServiceBus:SuscripcionSolicitudes.");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Un topic no se lee directo: cada suscripción recibe su propia copia del mensaje.
            await using var processor = _client.CreateProcessor(_topic, _suscripcion);
            processor.ProcessMessageAsync += ProcesarMensaje;
            processor.ProcessErrorAsync += ProcesarError;

            await processor.StartProcessingAsync(stoppingToken);

            try
            {
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Apagado normal de la aplicación.
            }

            await processor.StopProcessingAsync();
        }

        // Si termina bien, el processor completa el mensaje. Si lanza una excepción, lo abandona
        // y Service Bus lo reintenta hasta mandarlo a la dead-letter queue.
        private async Task ProcesarMensaje(ProcessMessageEventArgs args)
        {
            var errores = _validator.Validate(args.Message.Body.ToString(), out SolicitudReservaMensaje? solicitud);

            // Reintentar un mensaje inválido no lo arregla: va directo a la dead-letter queue.
            if (errores.Count > 0 || solicitud is null)
            {
                string detalle = string.Join(" ", errores);
                _logger.LogWarning("Mensaje {MessageId} inválido: {Errores}", args.Message.MessageId, detalle);
                await args.DeadLetterMessageAsync(args.Message, "MensajeInvalido", detalle, args.CancellationToken);
                return;
            }

            // El consumer es singleton e IBookingService es scoped: cada mensaje usa su propio scope.
            using var scope = _scopeFactory.CreateScope();
            var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();

            _logger.LogInformation("Solicitud {SolicitudId} recibida: evento {EventoId}, asiento {AsientoId}.",
                solicitud.SolicitudId, solicitud.EventoId, solicitud.AsientoId);

            var resultado = await bookingService.ProcessReservation(solicitud.SolicitudId, solicitud.EventoId, solicitud.AsientoId, solicitud.UsuarioId);

            _logger.LogInformation("Solicitud {SolicitudId} procesada: reserva {ReservaId}, estado {Estado}, motivo {Motivo}.",
                solicitud.SolicitudId, resultado.id, resultado.status, resultado.RejectionMotive);
        }

        private Task ProcesarError(ProcessErrorEventArgs args)
        {
            _logger.LogError(args.Exception, "Error de Service Bus en {Origen} ({Cola}).",
                args.ErrorSource, args.EntityPath);
            return Task.CompletedTask;
        }
    }
}
