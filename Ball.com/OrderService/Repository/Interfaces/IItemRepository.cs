using OrderService.Domain;

namespace OrderService.Repository.Interfaces
{
	public interface IItemRepository
	{
		Task<Item> GetByIdAsync(Guid id);
	}
}
