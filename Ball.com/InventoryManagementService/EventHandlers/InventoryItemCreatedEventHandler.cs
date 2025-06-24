using InventoryManagementService.Events;
using InventoryManagementService.Services;
using Shared.Infrastructure.Messaging.Events.Interfaces;
using Shared.Infrastructure.Messaging.Events.Items;

namespace InventoryManagementService.EventHandlers
{
    public class InventoryItemCreatedEventHandler : IEventHandler<ItemCreatedEvent>
    {
        private readonly ILogger<InventoryItemCreatedEventHandler> _logger;
        private readonly IReadModelUpdater _readModelUpdater;
        public InventoryItemCreatedEventHandler(ILogger<InventoryItemCreatedEventHandler> logger, IReadModelUpdater readModelUpdater)
        {
            _logger = logger;
            _readModelUpdater = readModelUpdater;
        }


        public async Task HandleAsync(ItemCreatedEvent @event)
        {

            _logger.LogInformation("Handling ItemCreatedEvent for ProductId: {ProductId}", @event.ItemId);
            var itemDomainEvent = new ItemCreatedDomainEvent
            {
                ItemId = @event.ItemId,
                Name = @event.Name,
                Description = @event.Description,
                Price = @event.Price,
                Stock = @event.Stock
            };
            // Update the read model with the new item details
            await _readModelUpdater.ApplyAsync(itemDomainEvent);
        }
    }
}
