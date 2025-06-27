using InventoryManagementService.Events;
using InventoryManagementService.Repositories;
using Shared.Infrastructure.Messaging.Events.Items;
using Shared.Infrastructure.Messaging.Interfaces;

namespace InventoryManagementService.Commands.Handlers
{
    public class CreateItemCommandHandler : ICommandHandler<CreateItemCommand>
    {
        private readonly IEventStore _eventStore;
        private readonly IEventPublisher _publisher;

        public CreateItemCommandHandler(IEventStore eventStore, IEventPublisher publisher)
        {
            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
            _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
        }

        public async Task HandleAsync(CreateItemCommand command)
        {
            var @event = new ItemCreatedDomainEvent
            {
                ItemId = command.ItemId,
                Name = command.Name,
                Description = command.Description,
                Price = command.Price,
                Stock = command.Stock
            };

            await _eventStore.SaveAsync(@event);

            // Create a new item instance and apply the event to it
            var item = new ItemCreatedEvent
            {
                Id = Guid.NewGuid(),
                ItemId = command.ItemId,
                Name = command.Name,
                Description = command.Description,
                Price = command.Price,
                Stock = command.Stock
            };



            await _publisher.PublishAsync(item);
        }
    }
}
