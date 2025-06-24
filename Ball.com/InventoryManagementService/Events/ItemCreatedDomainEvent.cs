namespace InventoryManagementService.Events
{
    public class ItemCreatedDomainEvent : ItemDomainEvent
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}
