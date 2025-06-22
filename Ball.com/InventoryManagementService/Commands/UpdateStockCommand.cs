namespace InventoryManagementService.Commands
{
    public class UpdateStockCommand : ICommand
    {
        public Guid ProductId { get; set; }
        public int Amount { get; set; }
        public UpdateStockCommand(Guid productId, int amount)
        {
            ProductId = productId;
            Amount = amount;
        }
    }
}
