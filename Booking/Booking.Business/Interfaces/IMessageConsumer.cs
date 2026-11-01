namespace Booking.Booking.Business.Interfaces
{
    public interface IMessageConsumer
    {
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
    }
}
