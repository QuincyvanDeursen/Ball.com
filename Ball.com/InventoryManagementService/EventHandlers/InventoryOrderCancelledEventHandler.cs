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
        private readonly IQueryHandler<GetItemsByIdQuery, ItemReadModel?> _queryHandler;
        private readonly IEventStore _eventStore;
        private readonly IEventPublisher _eventPublisher;

        public InventoryOrderCancelledEventHandler(
            IQueryHandler<GetItemsByIdQuery, ItemReadModel?> queryHandler,
            IEventStore eventStore,
            IEventPublisher eventPublisher)
        {
            _queryHandler = queryHandler;
            _eventStore = eventStore;
            _eventPublisher = eventPublisher;
        }

        public async Task HandleAsync(OrderCancelledEvent @event)
        {
            foreach (var item in @event.Items)
            {
                var getItemByIdQuery = new GetItemsByIdQuery
                {
                    Id = item.ItemId
                };

                var itemReadModel = await _queryHandler.HandleAsync(getItemByIdQuery);

                if (itemReadModel == null || itemReadModel.Stock < item.OrderQuantity)
                {
                    // Optionally: handle insufficient stock (e.g., throw, log, or publish a failure event)
                    continue;
                }

                var stockUpdatedDomainEvent = new StockUpdatedDomainEvent
                {
                    ItemId = item.ItemId,

                    Amount = item.OrderQuantity, // positive value to increase stock
                };


                await _eventStore.SaveAsync(stockUpdatedDomainEvent);

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
}
