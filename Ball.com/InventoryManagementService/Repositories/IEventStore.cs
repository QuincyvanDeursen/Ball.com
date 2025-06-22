using InventoryManagementService.Events;

namespace InventoryManagementService.Repositories
{
    public interface IEventStore
    {
        Task SaveAsync(ProductEvent @event);
        Task<List<ProductEvent>> GetEventsAsync(Guid productId);
    }
}
