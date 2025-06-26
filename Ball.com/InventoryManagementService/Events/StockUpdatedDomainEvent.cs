namespace InventoryManagementService.Events
{
    // DomainEvents are used to communicate changes in the domain model by event sourcing.
    public class StockUpdatedDomainEvent : ItemDomainEvent
    {
        public int Amount { get; set; }
    }
}
