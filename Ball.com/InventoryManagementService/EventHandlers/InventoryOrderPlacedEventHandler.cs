using InventoryManagementService.Events;
using InventoryManagementService.Models;
using InventoryManagementService.Queries;
using InventoryManagementService.Queries.Handlers;
using InventoryManagementService.Repositories;
using Shared.Infrastructure.Messaging.Events.Interfaces;
using Shared.Infrastructure.Messaging.Events.Items;
using Shared.Infrastructure.Messaging.Events.Orders;
using Shared.Infrastructure.Messaging.Interfaces;

namespace InventoryManagementService.EventHandlers
{
    public class InventoryOrderPlacedEventHandler : IEventHandler<OrderPlacedEvent>
    {
        private readonly IQueryHandler<GetItemsByIdQuery, ItemReadModel> _queryHandler;
        private readonly IEventStore _eventStore;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILogger<InventoryOrderPlacedEventHandler> _logger;

        public InventoryOrderPlacedEventHandler(
            IQueryHandler<GetItemsByIdQuery, ItemReadModel> queryHandler,
            IEventStore eventStore,
            IEventPublisher eventPublisher,
            ILogger<InventoryOrderPlacedEventHandler> logger)
        {
            _queryHandler = queryHandler ?? throw new ArgumentNullException(nameof(queryHandler));
            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task HandleAsync(OrderPlacedEvent @event)
        {
            _logger.LogInformation("Received OrderPlacedEvent from the message broker with OrderId: {OrderId}", @event.OrderId);

            foreach (var item in @event.Items)
            {
                await ProcessOrderItemAsync(item);
                _logger.LogInformation("Processed OrderItem with ItemId: {ItemId} for OrderId: {OrderId}", item.ItemId, @event.OrderId);
            }
        }

        private async Task ProcessOrderItemAsync(OrderItemDto item)
        {
            //Getting the item by Id from the read model
            var getItemByIdQuery = new GetItemsByIdQuery {Id = item.ItemId};
            var itemReadModel = await _queryHandler.HandleAsync(getItemByIdQuery);
            if (itemReadModel == null)
            {
                _logger.LogWarning("Item with ItemId: {ItemId} not found in the read model.", item.ItemId);
                return; // Skip processing if item does not exist
            }

            // Check if the item exists and has sufficient stock
            if (itemReadModel.Stock < item.OrderQuantity)
            {
                // @TODO
                _logger.LogWarning("Insufficient stock for ItemId: {ItemId}. Requested: {Requested}, Available: {Available}",
                    item.ItemId, item.OrderQuantity, itemReadModel?.Stock ?? 0);
            }

            // Creating a StockUpdatedDomainEvent to update the stock in the read model 
            var stockUpdatedDomainEvent = new StockUpdatedDomainEvent
            {
                ItemId = item.ItemId,
                Amount = item.OrderQuantity * -1 // Negative value to decrease stock
            };

            // Saving the stock updated domain event to the event store
            await _eventStore.SaveAsync(stockUpdatedDomainEvent);

            // Creating a StockUpdatedEvent to publish the stock update
            var stockUpdatedEvent = new StockUpdatedEvent
            {
                ItemId = itemReadModel!.Id,
                Amount = item.OrderQuantity,
                OccurredOn = DateTime.UtcNow
            };

            // Publishing the stock updated event to notify all the services
            await _eventPublisher.PublishAsync(stockUpdatedEvent);
        }
    }
}
