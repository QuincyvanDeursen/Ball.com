using OrderService.Domain;

namespace OrderService.Services.Interfaces
{
	public interface ICustomerService
	{
		Task<Customer> Get(Guid id);
	}
}
