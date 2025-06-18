using CustomerService.Domain;
using CustomerService.Dto;
using CustomerService.Repository.Interfaces;
using CustomerService.Services.Interfaces;


namespace CustomerService.Services
{

    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepo _customerRepo;
        //private readonly IMessagePublisher _publisher;

        public CustomerService(
            ICustomerRepo customerRepo
           )
        {
            _customerRepo = customerRepo;

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
            //await _publisher.PublishAsync(
            //    new CustomerDeletedEvent(id, DateTime.UtcNow),
            //    "customer.deleted");
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

            //await _publisher.PublishAsync(
            //    new CustomerCreatedEvent(
            //        customer.Id,
            //        customer.FirstName,
            //        customer.LastName,
            //        customer.Email,
            //        customer.Address,
            //        customer.PhoneNumber,
            //        DateTime.UtcNow),
            //    "customer.created");
        }

        public async Task Update(Guid id, CustomerUpdateDto dto)
        {
            var customer = await _customerRepo.GetCustomer(id)
                ?? throw new KeyNotFoundException($"Customer with ID {id} not found.");
            customer.PhoneNumber = dto.PhoneNumber ?? customer.PhoneNumber;
            customer.Address = dto.Address ?? customer.Address;
            customer.Email = dto.Email ?? customer.Email;

            await _customerRepo.UpdateCustomer(customer);

            //await _publisher.PublishAsync(
            //    new CustomerCreatedEvent(
            //        customer.Id,
            //        customer.FirstName,
            //        customer.LastName,
            //        customer.Email,
            //        customer.Address,
            //        customer.PhoneNumber,
            //        DateTime.UtcNow),
            //    "customer.updated");
        }
    }
}
