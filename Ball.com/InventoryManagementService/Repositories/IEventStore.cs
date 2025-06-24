using InventoryManagementService.Events;

namespace InventoryManagementService.Repositories
{
    public interface IEventStore
    {
        Task SaveAsync(ItemDomainEvent @event);
        Task<List<ItemDomainEvent>> GetEventsAsync(Guid productId);
    }
}
