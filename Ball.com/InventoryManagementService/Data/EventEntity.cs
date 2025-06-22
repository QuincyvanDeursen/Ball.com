namespace InventoryManagementService.Data
{
    public class EventEntity
    {
        public Guid Id { get; set; }
        public Guid AggregateId { get; set; }
        public string EventType { get; set; } = null!;
        public string Data { get; set; } = null!;
        public DateTime Timestamp { get; set; }
    }
}
