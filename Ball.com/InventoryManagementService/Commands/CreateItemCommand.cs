namespace InventoryManagementService.Commands
{
    public class CreateItemCommand : ICommand
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public int InitialStock { get; set; }
    }
}
