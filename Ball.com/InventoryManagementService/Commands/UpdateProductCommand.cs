namespace InventoryManagementService.Commands
{
    public class UpdateProductCommand : ICommand
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public UpdateProductCommand(Guid productId, string name, string description, decimal price)
        {
            ProductId = productId;
            Name = name;
            Description = description;
            Price = price;
        }
    }
}
