namespace InventoryManagementService.Events
{
    public abstract class ProductEvent
    {
        public Guid ProductId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    }
}
