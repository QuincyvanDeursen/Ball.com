
using InventoryManagementService.Events;
using InventoryManagementService.Repositories;
using Shared.Infrastructure.Messaging.Events.Items;
using Shared.Infrastructure.Messaging.Interfaces;

namespace InventoryManagementService.Commands.Handlers
{
    public class UpdateStockCommandHandler : ICommandHandler<UpdateStockCommand>
    {
        private readonly IEventStore _eventStore;
        private readonly IEventPublisher _eventPublisher;
        public UpdateStockCommandHandler(IEventStore eventStore, IEventPublisher publisher)
        {
            _eventStore = eventStore;
            _eventPublisher = publisher;
        }
        public async Task HandleAsync(UpdateStockCommand command)
        {
            //Event sourcing
            var @event = new StockUpdatedDomainEvent
            {
                ItemId = command.ItemId,
                Amount = command.Amount,
                Timestamp = DateTime.UtcNow
            };
            await _eventStore.SaveAsync(@event);

            //Publish event to RabbitMQ
            var UpdateStockEvent = new StockUpdatedEvent
            {
                ItemId = command.ItemId,
                Amount = command.Amount
            };
            await _eventPublisher.PublishAsync(UpdateStockEvent);
        }
    }
}
