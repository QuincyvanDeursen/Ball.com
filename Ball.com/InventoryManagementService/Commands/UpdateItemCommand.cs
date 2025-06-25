namespace InventoryManagementService.Commands
{
    public class UpdateItemCommand : ICommand
    {
        public Guid ItemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public UpdateItemCommand(Guid itemId, string name, string description, decimal price)
        {
            ItemId = itemId;
            Name = name;
            Description = description;
            Price = price;
        }
    }
}
