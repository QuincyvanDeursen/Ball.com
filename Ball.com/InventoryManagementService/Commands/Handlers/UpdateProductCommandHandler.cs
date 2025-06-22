using InventoryManagementService.Events;
using InventoryManagementService.Repositories;

namespace InventoryManagementService.Commands.Handlers
{
    public class UpdateProductCommandHandler : ICommandHandler<UpdateProductCommand>
    {
        private readonly IEventStore _eventStore;
        public UpdateProductCommandHandler(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }
        public async Task HandleAsync(UpdateProductCommand command)
        {
            var @event = new ProductUpdatedEvent
            {
                ProductId = command.ProductId,
                Name = command.Name,
                Description = command.Description,
                Price = command.Price,
                Timestamp = DateTime.UtcNow
            };
            await _eventStore.SaveAsync(@event);
        }
    }
}
