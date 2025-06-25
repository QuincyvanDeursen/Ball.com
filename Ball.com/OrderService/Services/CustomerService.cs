using OrderService.Domain;
using OrderService.Repository.Interfaces;
using OrderService.Services.Interfaces;

namespace OrderService.Services
{
	public class CustomerService : ICustomerService
	{
		private readonly ICustomerRepository _customerRepository;
		public CustomerService(ICustomerRepository customerRepository)
		{
			_customerRepository = customerRepository;
		}
		public async Task<Customer> Get(Guid id)
		{
			return await _customerRepository.GetByIdAsync(id);
		}
	}
}
