using CustomerService.Domain;
using CustomerService.Dto;
using CustomerService.Repository.Interfaces;
using CustomerService.Services.Interfaces;
using Shared.Infrastructure.Messaging.Events;
using Shared.Infrastructure.Messaging.Interfaces;


namespace CustomerService.Services
{

    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepo _customerRepo;
        private readonly IEventPublisher _eventPublisher;

        public CustomerService(ICustomerRepo customerRepo, IEventPublisher eventPublisher)
        {
            _customerRepo = customerRepo;
            _eventPublisher = eventPublisher;
        }

        public async Task<Customer> Get(Guid id)
        {
            var customer = await _customerRepo.GetCustomer(id);
            return customer ?? throw new KeyNotFoundException($"Customer with ID {id} not found.");
        }

        public async Task<IEnumerable<Customer>> GetAll()
        {
            var customers = await _customerRepo.GetAllCustomers();
            return customers?.Any() == true
                ? customers
                : throw new KeyNotFoundException("No customers found.");
        }

        public async Task Delete(Guid id)
        {
            await _customerRepo.DeleteCustomer(id);
        }

        public async Task Create(CustomerCreateDto dto)
        {
            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address
            };

            await _customerRepo.AddCustomer(customer);

        }

        public async Task Update(Guid id, CustomerUpdateDto dto)
        {
            var customer = await _customerRepo.GetCustomer(id)
                ?? throw new KeyNotFoundException($"Customer with ID {id} not found.");

            customer.PhoneNumber = dto.PhoneNumber ?? customer.PhoneNumber;
            customer.Address = dto.Address ?? customer.Address;
            customer.Email = dto.Email ?? customer.Email;

            await _customerRepo.UpdateCustomer(customer);

            // Maak event aan en publiceer
            var customerUpdatedEvent = new CustomerUpdatedEvent
            {
                CustomerId = customer.Id,
                PhoneNumber = customer.PhoneNumber,
                Email = customer.Email
            };

            await _eventPublisher.PublishAsync(customerUpdatedEvent);
        }
    }
}
