
using InventoryManagementService.Events;
using InventoryManagementService.Repositories;
using InventoryManagementService.Services;

namespace InventoryManagementService.Commands.Handlers
{
    public class UpdateStockCommandHandler : ICommandHandler<UpdateStockCommand>
    {
        private readonly IEventStore _eventStore;
        private readonly IReadModelUpdater _readModelUpdater;
        public UpdateStockCommandHandler(IEventStore eventStore, IReadModelUpdater readModelUpdater)
        {
            _eventStore = eventStore;
            _readModelUpdater = readModelUpdater;
        }
        public async Task HandleAsync(UpdateStockCommand command)
        {
            var @event = new StockUpdatedEvent
            {
                ProductId = command.ProductId,
                Amount = command.Amount,
                Timestamp = DateTime.UtcNow
            };
            await _eventStore.SaveAsync(@event);
            //await _readModelUpdater.ApplyAsync(@event);
        }
    }
}
