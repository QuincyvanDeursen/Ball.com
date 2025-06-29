using OrderService.Domain;
using OrderService.Dto;

namespace OrderService.Services.Interfaces
{
	public interface IOrderService
	{
		Task<IEnumerable<Order>> GetAll();
		Task<Order> Get(Guid id);
		Task Create(OrderCreateDto order);
		Task Update(OrderUpdateDto order);
		Task UpdateAndSendEvent(OrderUpdateDto order);
	}
}
