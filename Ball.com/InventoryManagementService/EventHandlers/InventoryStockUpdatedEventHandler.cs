using InventoryManagementService.Events;
using InventoryManagementService.Services;
using Shared.Infrastructure.Messaging.Events.Interfaces;
using Shared.Infrastructure.Messaging.Events.Items;

namespace InventoryManagementService.EventHandlers
{
    public class InventoryStockUpdatedEventHandler : IEventHandler<StockUpdatedEvent>
    {

        private readonly ILogger<InventoryStockUpdatedEventHandler> _logger;
        private readonly IReadModelUpdater _readModelUpdater;
        public InventoryStockUpdatedEventHandler(ILogger<InventoryStockUpdatedEventHandler> logger, IReadModelUpdater readModelUpdater)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _readModelUpdater = readModelUpdater ?? throw new ArgumentNullException(nameof(readModelUpdater));
        }

        public async Task HandleAsync(StockUpdatedEvent @event)
        {
            _logger.LogInformation("Received StockUpdatedEvent from the message broker for item with id {ItemId}", @event.ItemId);
            var stockUpdateDomainEvent = new StockUpdatedDomainEvent
            {
                ItemId = @event.ItemId,
                Amount = @event.Amount,
            };

            // Update the read model with the new item details (can be stock increase or decrease)
            await _readModelUpdater.ApplyAsync(stockUpdateDomainEvent);
        }
    }
}
