using InventoryManagementService.Events;
using InventoryManagementService.Repositories;
using Shared.Infrastructure.Messaging.Events.Items;
using Shared.Infrastructure.Messaging.Interfaces;

namespace InventoryManagementService.Commands.Handlers
{
    public class UpdateItemCommandHandler : ICommandHandler<UpdateItemCommand>
    {
        private readonly IEventStore _eventStore;
        private readonly IEventPublisher _eventPublisher;
        public UpdateItemCommandHandler(IEventStore eventStore, IEventPublisher eventPublisher)
        {
            _eventStore = eventStore;
            _eventPublisher = eventPublisher;
        }
        public async Task HandleAsync(UpdateItemCommand command)
        {
            var @event = new ItemUpdatedDomainEvent
            {
                ItemId = command.ProductId,
                Name = command.Name,
                Description = command.Description,
                Price = command.Price,
                Timestamp = DateTime.UtcNow
            };
            await _eventStore.SaveAsync(@event);

            var itemUpdatedEvent = new ItemUpdatedEvent
            {
                Id = command.ProductId,
                Name = command.Name,
                Description = command.Description,
                Price = command.Price
            };

            // Publish the event to notify other services or components
            await _eventPublisher.PublishAsync(itemUpdatedEvent);
        }
    }
}
