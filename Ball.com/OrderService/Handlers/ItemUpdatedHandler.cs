

using OrderService.Dto;
using OrderService.Services.Interfaces;
using Shared.Infrastructure.Messaging.Events.Interfaces;
using Shared.Infrastructure.Messaging.Events.Items;

namespace OrderService.Handlers
{
	public class ItemUpdatedHandler : IEventHandler<ItemUpdatedEvent>
	{
		private readonly IItemService _itemService;
		private readonly ILogger<ItemUpdatedHandler> _logger;

		public ItemUpdatedHandler(IItemService itemService, ILogger<ItemUpdatedHandler> logger)
		{
			_itemService = itemService;
			_logger = logger;
		}

		public async Task HandleAsync(ItemUpdatedEvent @event)
		{
			_logger.LogInformation("Handling Item.Updated event for ItemId: {ItemId}", @event.ItemId);

			if (await _itemService.Get(@event.ItemId) == null)
			{
				throw new KeyNotFoundException($"Item with ID {@event.ItemId} not found OrderService.");
			}

			ItemUpdateDto itemUpdateDto = new ItemUpdateDto
			{
				ItemId = @event.ItemId,
				Name = @event.Name,
				Price = @event.Price,
			};

			await _itemService.Update(itemUpdateDto);

			_logger.LogInformation("Item.Updated event processed successfully for ItemId: {ItemId}", @event.ItemId);
		}
	}
}
