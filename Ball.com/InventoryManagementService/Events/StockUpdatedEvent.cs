namespace InventoryManagementService.Events
{
    public class StockUpdatedEvent : ProductEvent
    {
        public int Amount { get; set; }
    }
}
