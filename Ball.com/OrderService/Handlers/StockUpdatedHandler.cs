using OrderService.Dto;
using OrderService.Services.Interfaces;
using Shared.Infrastructure.Messaging.Events.Interfaces;
using Shared.Infrastructure.Messaging.Events.Items;

namespace OrderService.Handlers
{
	public class StockUpdatedHandler : IEventHandler<StockUpdatedEvent>
	{
		private readonly IItemService _itemService;
		private readonly ILogger<StockUpdatedHandler> _logger;

		public StockUpdatedHandler(IItemService itemService, ILogger<StockUpdatedHandler> logger)
		{
			_itemService = itemService;
			_logger = logger;
		}

		public async Task HandleAsync(StockUpdatedEvent @event)
		{
			_logger.LogInformation("Handling Stock.Updated event for ItemId: {ItemId}", @event.ItemId);

			if (await _itemService.Get(@event.ItemId) == null)
			{
				throw new KeyNotFoundException($"Item with ID {@event.ItemId} not found OrderService.");
			}

			await _itemService.UpdateStock(@event.ItemId, @event.Amount);

			_logger.LogInformation("Stock.Updated event processed successfully for ItemId: {ItemId}", @event.ItemId);
		}
	}
}
