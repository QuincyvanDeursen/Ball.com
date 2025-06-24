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
            _logger = logger;
            _readModelUpdater = readModelUpdater;
        }

        public async Task HandleAsync(StockUpdatedEvent @event)
        {

            _logger.LogInformation("Handling ItemCreatedEvent for ProductId: {ProductId}", @event.ItemId);
            var stockUpdateDomainEvent = new StockUpdatedDomainEvent
            {
                ItemId = @event.ItemId,
                Amount = @event.Amount,
            };
            // Update the read model with the new item details
            await _readModelUpdater.ApplyAsync(stockUpdateDomainEvent);
        }
    }
}
