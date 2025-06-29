using OrderService.Domain;
using OrderService.Dto;
using OrderService.Services.Interfaces;
using Shared.Infrastructure.Messaging.Events.Interfaces;
using Shared.Infrastructure.Messaging.Events.Items;

namespace OrderService.Handlers
{
	public class ItemCreatedHandler : IEventHandler<ItemCreatedEvent>
	{
		private readonly IItemService _itemService;
		private readonly ILogger<ItemCreatedHandler> _logger;

		public ItemCreatedHandler(IItemService itemService, ILogger<ItemCreatedHandler> logger)
		{
			_itemService = itemService;
			_logger = logger;
		}

		public async Task HandleAsync(ItemCreatedEvent @event)
		{
			_logger.LogInformation("Handling Item.Created event for Itemid: {ItemId}", @event.ItemId);

			ItemCreateDto item;

			try
			{
				await _itemService.Get(@event.ItemId);
				_logger.LogInformation("Item with ID {ItemId} already exists. Skipping creation.", @event.ItemId);
				return;
			}
			catch (KeyNotFoundException)
			{
				item = new ItemCreateDto
				{
					ItemId = @event.ItemId,
					Price = @event.Price,
					Name = @event.Name
				};
			}

			await _itemService.Create(item);

			_logger.LogInformation("Item.Created event processed successfully for Itemid: {ItemId}", @event.ItemId);
		}
	}
}
