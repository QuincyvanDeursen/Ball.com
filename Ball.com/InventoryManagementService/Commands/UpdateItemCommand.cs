namespace InventoryManagementService.Commands
{
    public class UpdateItemCommand : ICommand
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public UpdateItemCommand(Guid productId, string name, string description, decimal price)
        {
            ProductId = productId;
            Name = name;
            Description = description;
            Price = price;
        }
    }
}
