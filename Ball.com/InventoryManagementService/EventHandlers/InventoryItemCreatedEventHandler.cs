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
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _readModelUpdater = readModelUpdater ?? throw new ArgumentNullException(nameof(readModelUpdater));
        }

        public async Task HandleAsync(ItemCreatedEvent @event)
        {
            _logger.LogInformation("Received ItemCreatedEvent from the message broker");

            //creating a new ItemCreatedDomainEvent to update the read model
            var itemDomainEvent = new ItemCreatedDomainEvent
            {
                ItemId = @event.ItemId,
                Name = @event.Name,
                Description = @event.Description,
                Price = @event.Price,
                Stock = @event.Stock
            };

            // Create the read model using the read model updater service
            await _readModelUpdater.ApplyAsync(itemDomainEvent);
        }
    }
}
