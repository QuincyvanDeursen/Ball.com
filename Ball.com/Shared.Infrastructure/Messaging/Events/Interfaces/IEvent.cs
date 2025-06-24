namespace Shared.Infrastructure.Messaging.Events.Interfaces
{
    public interface IEvent
    {
        Guid Id { get; }
        DateTime OccurredOn { get; }
        string EventType { get; }
    }
}
