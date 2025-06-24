namespace InventoryManagementService.Events
{
    public abstract class ItemDomainEvent
    {
        public Guid ItemId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    }
}
