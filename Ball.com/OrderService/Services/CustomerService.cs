using ItemService.Repository;
using OrderService.Domain;
using OrderService.Dto;
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

		public async Task Create(CustomerCreateDto customerDto)
		{
			var customer = new Customer
			{
				CustomerId = customerDto.CustomerId,
				Address = customerDto.Address,
				Email = customerDto.Email,
				PhoneNumber = customerDto.PhoneNumber,
				FirstName = customerDto.FirstName,
				LastName = customerDto.LastName,
			};

			await _customerRepository.CreateAsync(customer);
		}

		public async Task<Customer> Get(Guid id)
		{
			return await _customerRepository.GetByIdAsync(id);
		}

		public async Task Update(CustomerUpdateDto customerDto)
		{
			var oldCustomer = await _customerRepository.GetByIdAsync(customerDto.CustomerId);
			if (oldCustomer == null) throw new KeyNotFoundException($"Customer with ID {customerDto.CustomerId} not found");
			var customer = new Customer
			{
				CustomerId = customerDto.CustomerId,
				Address = customerDto.Address,
				Email = customerDto.Email,
				PhoneNumber = customerDto.PhoneNumber,
				FirstName = oldCustomer.FirstName,
				LastName = oldCustomer.LastName,
			};
			await _customerRepository.UpdateAsync(customer);
		}
	}
}
