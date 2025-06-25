using OrderService.Domain;

namespace OrderService.Services.Interfaces
{
	public interface IItemService
	{
		Task<Item> Get(Guid id);
	}
}
