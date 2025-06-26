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
    public class InventoryOrderCancelledEventHandler : IEventHandler<OrderCancelledEvent>
    {   
        private readonly ILogger<InventoryOrderCancelledEventHandler> _logger;
        private readonly IQueryHandler<GetItemsByIdQuery, ItemReadModel> _queryHandler;
        private readonly IEventStore _eventStore;
        private readonly IEventPublisher _eventPublisher;

        public InventoryOrderCancelledEventHandler(
            IQueryHandler<GetItemsByIdQuery, ItemReadModel> queryHandler,
            IEventStore eventStore,
            IEventPublisher eventPublisher,
            ILogger<InventoryOrderCancelledEventHandler> logger)
        {
            _queryHandler = queryHandler ?? throw new ArgumentNullException(nameof(queryHandler));
            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
            _logger = logger;
        }

        public async Task HandleAsync(OrderCancelledEvent @event)
        {
            _logger.LogInformation("Received OrderCancelledEvent from the message broker with OrderId: {OrderId}", @event.OrderId);

            foreach (var item in @event.Items)
            {
                await ProcessCancelledOrderItemAsync(item);
                _logger.LogInformation("Processed Cancelled OrderItem with ItemId: {ItemId} for OrderId: {OrderId}", item.ItemId, @event.OrderId);
            }
        }

        private async Task ProcessCancelledOrderItemAsync(OrderItemDto item)
        {
            var getItemByIdQuery = new GetItemsByIdQuery { Id = item.ItemId };
            var itemReadModel = await _queryHandler.HandleAsync(getItemByIdQuery);

            if (itemReadModel == null)
            {
                _logger.LogWarning("Item with ItemId: {ItemId} not found in the read model.", item.ItemId);
                return; 
            }

            // The stock is increased by the order quantity when an order is cancelled (to reflect the cancellation)
            var stockUpdatedDomainEvent = new StockUpdatedDomainEvent
            {
                ItemId = item.ItemId,
                Amount = item.OrderQuantity // Positive value to increase stock
            };

            await _eventStore.SaveAsync(stockUpdatedDomainEvent);


            // Publish a StockUpdatedEvent to notify other services of the stock update
            var stockUpdatedEvent = new StockUpdatedEvent
            {
                ItemId = itemReadModel.Id,
                Amount = item.OrderQuantity,
                OccurredOn = DateTime.UtcNow
            };

            await _eventPublisher.PublishAsync(stockUpdatedEvent);
        }
    }
}
