namespace Shared.Infrastructure.Messaging.Interfaces
{
    public interface IEventDispatcher
    {
        Task DispatchAsync(string eventType, string payload);
    }

}
