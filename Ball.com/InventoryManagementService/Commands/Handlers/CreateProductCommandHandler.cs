using InventoryManagementService.Events;
using InventoryManagementService.Repositories;
using InventoryManagementService.Services;

namespace InventoryManagementService.Commands.Handlers
{
    public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand>
    {
        private readonly IEventStore _eventStore;
        private readonly IReadModelUpdater _readModelUpdater;

        public CreateProductCommandHandler(IEventStore eventStore, IReadModelUpdater readModelUpdater)
        {
            _eventStore = eventStore;
            _readModelUpdater = readModelUpdater;
        }

        public async Task HandleAsync(CreateProductCommand command)
        {
            var @event = new ProductCreated
            {
                ProductId = command.Id,
                Name = command.Name,
                Description = command.Description,
                Price = command.Price,
                InitialStock = command.InitialStock
            };

            await _eventStore.SaveAsync(@event);
            await _readModelUpdater.ApplyAsync(@event);
        }
    }
}
