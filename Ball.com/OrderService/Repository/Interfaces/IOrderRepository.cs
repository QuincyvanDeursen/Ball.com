using OrderService.Domain;

namespace OrderService.Repository.Interfaces
{
	public interface IOrderRepository
	{
		Task<Order> GetByIdAsync(Guid id);
		Task<IEnumerable<Order>> GetAllAsync();
		Task CreateAsync(Order order);
		Task UpdateAsync(Order order);
	}
}
