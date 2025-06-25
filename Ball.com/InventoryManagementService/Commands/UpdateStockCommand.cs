namespace InventoryManagementService.Commands
{
    public class UpdateStockCommand : ICommand
    {
        public Guid ItemId { get; set; }
        public int Amount { get; set; }
        public UpdateStockCommand(Guid itemId, int amount)
        {
            ItemId = itemId;
            Amount = amount;
        }
    }
}
