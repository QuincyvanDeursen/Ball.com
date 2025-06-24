namespace Shared.Infrastructure.Messaging.Events.Items
{
    public class StockUpdatedEvent : BaseEvent
    {
        public override string EventType => "stock.updated";
        public Guid ItemId { get; init; }
        public int Amount { get; init; }

    }
}
