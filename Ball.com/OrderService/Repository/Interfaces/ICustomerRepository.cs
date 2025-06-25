using OrderService.Domain;

namespace OrderService.Repository.Interfaces
{
	public interface ICustomerRepository
	{
		Task<Customer> GetByIdAsync(Guid id);
	}
}
