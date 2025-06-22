using InventoryManagementService.Events;

namespace InventoryManagementService.Services
{
    public interface IReadModelUpdater
    {
        Task ApplyAsync(ProductEvent @event);
    }
}
