using OrderService.Domain;
using OrderService.Dto;
using Shared.Infrastructure.Messaging.Events.Orders;

namespace OrderService.Services.Interfaces
{
	public interface IItemService
	{
		Task<Item> Get(Guid id);
		Task Create(ItemCreateDto item);
		Task Update(ItemUpdateDto item);
		Task UpdateStock(Guid id, int amount);
	}
}
