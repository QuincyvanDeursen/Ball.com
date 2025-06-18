namespace Shared.Infrastructure.Messaging.Interfaces
{
    public interface IEventConsumer
    {
        Task ConsumeAsync(CancellationToken cancellationToken);
    }
}
