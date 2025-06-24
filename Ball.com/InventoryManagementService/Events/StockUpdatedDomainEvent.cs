namespace InventoryManagementService.Events
{
    public class StockUpdatedDomainEvent : ItemDomainEvent
    {
        public int Amount { get; set; }
    }
}
