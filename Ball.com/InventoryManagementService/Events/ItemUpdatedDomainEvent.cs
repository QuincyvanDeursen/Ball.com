namespace InventoryManagementService.Events
{
    public class ItemUpdatedDomainEvent : ItemDomainEvent
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
    }
}
