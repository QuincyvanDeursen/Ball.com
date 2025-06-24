namespace Shared.Infrastructure.Messaging.Events.Items
{
    public class ItemCreatedEvent : BaseEvent
    {
        public override string EventType => "Item.Created";
        public Guid ItemId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public int Stock { get; set; }

    }
}
