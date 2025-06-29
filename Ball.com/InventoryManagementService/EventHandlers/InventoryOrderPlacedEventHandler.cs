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

            bool validOrder = true;
            foreach (var item in @event.Items)
            {
                if (!await ValidateOrderItemAsync(item))
                {
                    validOrder = false;
                    break;
                }
                _logger.LogInformation("Validated OrderItem with ItemId: {ItemId} for OrderId: {OrderId}", item.ItemId, @event.OrderId);
            }
            if (validOrder)
            {
                foreach (var item in @event.Items)
                {
                    await ProcessOrderItemAsync(item);
                }
                OrderValidatedEvent orderValidatedEvent = new OrderValidatedEvent
                {
                    OrderId = @event.OrderId,
                    CustomerId = @event.CustomerId,
                    OrderDate = @event.OrderDate,
                    TotalPrice = @event.TotalPrice,
                    FirstName = @event.FirstName,
                    LastName = @event.LastName,
                    Email = @event.Email,
                    Address = @event.Address,
                    PhoneNumber = @event.PhoneNumber,
                    Items = @event.Items
                };
				await _eventPublisher.PublishAsync(orderValidatedEvent);
				_logger.LogInformation("Order processed for Order with OrderId {OrderId}", @event.OrderId);
			} else {
                OrderCancelledEvent orderCancelledEvent = new OrderCancelledEvent
                {
                    OrderId = @event.OrderId,
                    CustomerId = @event.CustomerId,
                    OrderDate = @event.OrderDate,
                    TotalPrice = @event.TotalPrice,
                    FirstName = @event.FirstName,
                    LastName = @event.LastName,
                    Email = @event.Email,
                    Address = @event.Address,
                    PhoneNumber = @event.PhoneNumber,
                    Items = @event.Items
                };
				await _eventPublisher.PublishAsync(orderCancelledEvent);
			}
		}

        private async Task<bool> ValidateOrderItemAsync(OrderItemDto item)
        {
			//Getting the item by Id from the read model
			var getItemByIdQuery = new GetItemsByIdQuery { Id = item.ItemId };
			var itemReadModel = await _queryHandler.HandleAsync(getItemByIdQuery);
			if (itemReadModel == null)
			{
				_logger.LogWarning("Item with ItemId: {ItemId} not found in the read model.", item.ItemId);
				return false; // Item doesnt exist, order is invalid
			}

			// Check if the item exists and has sufficient stock
			if (itemReadModel.Stock < item.OrderQuantity)
			{
				_logger.LogWarning("Insufficient stock for ItemId: {ItemId}. Requested: {Requested}, Available: {Available}",
					item.ItemId, item.OrderQuantity, itemReadModel?.Stock ?? 0);
				return false;
			}
            return true;
		}

        private async Task ProcessOrderItemAsync(OrderItemDto item)
        {
			//Getting the item by Id from the read model
			var getItemByIdQuery = new GetItemsByIdQuery { Id = item.ItemId };
			var itemReadModel = await _queryHandler.HandleAsync(getItemByIdQuery);
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
        }
    }
}
