using InventoryManagementService.Events;
using InventoryManagementService.Models;

namespace InventoryManagementService.Services
{
    public interface IReadModelUpdater
    {
        Task ApplyAsync(ItemDomainEvent @event);
        Task<ItemReadModel?> RestoreAndSave(IEnumerable<ItemDomainEvent> events);
    }
}
