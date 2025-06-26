namespace InventoryManagementService.Events
{
    // DomainEvents are used to communicate changes in the domain model by event sourcing.
    public abstract class ItemDomainEvent
    {
        public Guid ItemId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    }
}
