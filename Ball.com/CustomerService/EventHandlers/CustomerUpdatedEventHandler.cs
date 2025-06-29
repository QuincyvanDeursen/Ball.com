using System;
using System.Threading.Tasks;
using CustomerService.Repository.Interfaces;
using Shared.Infrastructure.Messaging.Events;
using Shared.Infrastructure.Messaging.Events.Interfaces;
using Shared.Infrastructure.Messaging.Interfaces;

namespace CustomerService.EventHandlers
{
    public class CustomerUpdatedEventHandler : IEventHandler<CustomerUpdatedEvent>
    {
        private readonly ICustomerRepo _repo;

        public CustomerUpdatedEventHandler(ICustomerRepo repo)
            => _repo = repo;

        public async Task HandleAsync(CustomerUpdatedEvent @event)
        {
            var customer = await _repo.GetCustomer(@event.CustomerId)
                          ?? throw new KeyNotFoundException($"Customer {@event.CustomerId} not found.");

            customer.PhoneNumber = @event.PhoneNumber;
            customer.Email       = @event.Email;
            customer.Address     = @event.Address;
            // If you added CompanyName to your model/DTO, uncomment:
            // customer.CompanyName = @event.CompanyName;

            await _repo.UpdateCustomer(customer);
        }
    }
}
