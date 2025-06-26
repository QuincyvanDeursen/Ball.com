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
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _readModelUpdater = readModelUpdater ?? throw new ArgumentNullException(nameof(readModelUpdater));
        }


        public async Task HandleAsync(ItemUpdatedEvent @event)
        {
            _logger.LogInformation("Received ItemUpdatedEvent from the message broker with itemId {ItemId}", @event.ItemId);

            //creating a new ItemUpdatedDomainEvent to update the read model
            var itemDomainEvent = new ItemUpdatedDomainEvent
            {
                ItemId = @event.ItemId,
                Name = @event.Name,
                Description = @event.Description,
                Price = @event.Price,
            };
            // Update the read model with the new item details
            await _readModelUpdater.ApplyAsync(itemDomainEvent);
            _logger.LogInformation("Updated the readmodel for item with id {ItemId}", @event.ItemId);
        }
    }
}
