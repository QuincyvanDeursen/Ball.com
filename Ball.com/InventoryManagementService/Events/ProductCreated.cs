namespace InventoryManagementService.Events
{
    public class ProductCreated : ProductEvent
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public int InitialStock { get; set; }
    }
}
