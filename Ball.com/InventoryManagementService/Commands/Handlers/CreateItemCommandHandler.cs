using InventoryManagementService.Events;
using InventoryManagementService.Models;
using InventoryManagementService.Repositories;
using InventoryManagementService.Services;
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
            _eventStore = eventStore;
            _publisher = publisher;
        }

        public async Task HandleAsync(CreateItemCommand command)
        {
            var @event = new ItemCreatedDomainEvent
            {
                ItemId = command.Id,
                Name = command.Name,
                Description = command.Description,
                Price = command.Price,
                Stock = command.InitialStock
            };

            // Create a new item instance and apply the event to it
            var item = new ItemCreatedEvent
            {
                Id = command.Id,
                Name = command.Name,
                Description = command.Description,
                Price = command.Price,
                Stock = command.InitialStock
            };


            await _eventStore.SaveAsync(@event);
            await _publisher.PublishAsync(item);
        }
    }
}
