namespace InventoryManagementService.Data
{
    // This class represents an event entity in the event sourcing system.
    // It contains properties that define the event's identity, type, data, and timestamp.
    // Dat is the serialized representation of the event data, which can be deserialized into a specific event type later.
    public class EventEntity
    {
        public Guid Id { get; set; }
        public Guid AggregateId { get; set; }
        public string EventType { get; set; } = null!;
        public string Data { get; set; } = null!;
        public DateTime Timestamp { get; set; }
    }
}
