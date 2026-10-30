using Grpc.Core;
using SistemaReservas.Contracts.Payment.V1;

namespace Payment.Grpc.Services
{
    // Capa de Presentación (N-Layer): punto de entrada gRPC de Payment.
    // Hereda de la clase base que genera Grpc.Tools a partir de Contract/gRPC/payment.proto.
    // Solo traduce entre el formato del contrato y Business; no toma decisiones de negocio.
    public class PaymentGrpcService : PaymentService.PaymentServiceBase
    {
        public override Task<PaymentResponseDto> ProcesarPago(PaymentRequestDto request, ServerCallContext context)
        {
            // TEMPORAL: respuesta fija para verificar que el servidor gRPC funciona.
            // Se reemplaza por la llamada a PaymentBusiness cuando exista esa capa.
            var response = new PaymentResponseDto
            {
                Exitoso = true,
                Mensaje = $"Respuesta de prueba para la reserva {request.ReservaId}",
                CodigoTransferencia = "PRUEBA"
            };

            return Task.FromResult(response);
        }
    }
}