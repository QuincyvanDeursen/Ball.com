using System.Threading.Tasks;
using CustomerService.Domain;
using CustomerService.Repository.Interfaces;
using Shared.Infrastructure.Messaging.Events;
using Shared.Infrastructure.Messaging.Events.Interfaces;
using Shared.Infrastructure.Messaging.Interfaces;

namespace CustomerService.EventHandlers
{
    public class CustomerCreatedEventHandler : IEventHandler<CustomerCreatedEvent>
    {
        private readonly ICustomerRepo _repo;

        public CustomerCreatedEventHandler(ICustomerRepo repo)
            => _repo = repo;

        public async Task HandleAsync(CustomerCreatedEvent @event)
        {
            var customer = new Customer
            {
                Id          = @event.CustomerId,
                FirstName   = @event.FirstName,
                LastName    = @event.LastName,
                PhoneNumber = @event.PhoneNumber,
                Email       = @event.Email,
                Address     = @event.Address
                // If you added CompanyName to your model/DTO, uncomment:
                // CompanyName = @event.CompanyName
            };

            await _repo.AddCustomer(customer);
        }
    }
}
