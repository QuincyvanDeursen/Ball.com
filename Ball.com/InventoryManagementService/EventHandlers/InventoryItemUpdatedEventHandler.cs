using InventoryManagementService.Events;
using InventoryManagementService.Services;
using Shared.Infrastructure.Messaging.Events.Interfaces;
using Shared.Infrastructure.Messaging.Events.Items;

namespace InventoryManagementService.EventHandlers
{
    public class InventoryItemUpdatedEventHandler : IEventHandler<ItemUpdatedEvent>
    {
        private readonly ILogger<InventoryItemUpdatedEventHandler> _logger;
        private readonly IReadModelUpdater _readModelUpdater;
        public InventoryItemUpdatedEventHandler(ILogger<InventoryItemUpdatedEventHandler> logger, IReadModelUpdater readModelUpdater)
        {
            _logger = logger;
            _readModelUpdater = readModelUpdater;
        }


        public async Task HandleAsync(ItemUpdatedEvent @event)
        {

            _logger.LogInformation("Handling ItemUpdatedEvent for ProductId: {ProductId}", @event.ItemId);
            var itemDomainEvent = new ItemCreatedDomainEvent
            {
                ItemId = @event.ItemId,
                Name = @event.Name,
                Description = @event.Description,
                Price = @event.Price,
            };
            // Update the read model with the new item details
            await _readModelUpdater.ApplyAsync(itemDomainEvent);
        }
    }
}
