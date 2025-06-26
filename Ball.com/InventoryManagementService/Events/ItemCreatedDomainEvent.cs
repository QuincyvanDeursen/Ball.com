namespace InventoryManagementService.Events
{
    // DomainEvents are used to communicate changes in the domain model by event sourcing.
    public class ItemCreatedDomainEvent : ItemDomainEvent
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}
