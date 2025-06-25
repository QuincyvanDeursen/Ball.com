using OrderService.Domain;
using OrderService.Dto;

namespace OrderService.Services.Interfaces
{
	public interface ICustomerService
	{
		Task<Customer> Get(Guid id);
		Task Create(CustomerCreateDto customer);
		Task Update(CustomerUpdateDto customer);
	}
}
