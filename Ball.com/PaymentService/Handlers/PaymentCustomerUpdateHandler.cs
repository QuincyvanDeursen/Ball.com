using PaymentService.Repository.Interfaces;
using Shared.Infrastructure.Messaging.Events;
using Shared.Infrastructure.Messaging.Events.Interfaces;

namespace PaymentService.Handlers
{
    public class PaymentCustomerUpdateHandler : IEventHandler<CustomerUpdatedEvent>
    {

        // Deze handler luistert naar het CustomerUpdatedEvent en werkt de klantgegevens bij in de PaymentService database.
        // De reden dat we dit doen is omdat paymentService geen directe database connectie heeft met de CustomerService.
        // Er wordt dus een kopie van de klantgegevens opgeslagen in de PaymentService database die wordt bijgewerkt wanneer de klant zijn gegevens wijzigt.
        // We passen niet de Payment records aan, omdat deze de historische gegevens van de klant bevatten op het moment van betaling.

        private readonly ICustomerRepo _repo;
        private readonly ILogger<PaymentCustomerUpdateHandler> _logger;

        public PaymentCustomerUpdateHandler(ICustomerRepo repo, ILogger<PaymentCustomerUpdateHandler> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task HandleAsync(CustomerUpdatedEvent @event)
        {
            _logger.LogInformation("Handling Customer.Updated event for CustomerId: {CustomerId}", @event.CustomerId);
            var customer = await _repo.GetByIdAsync(@event.CustomerId);
            
            if (customer == null)
            {
                // Als de klant niet bestaat, log een waarschuwing of gooi een uitzondering
                throw new KeyNotFoundException($"Customer with ID {@event.CustomerId} not found PaymentService.");
            }

            // Update de klantgegevens in de PaymentService database
            customer.Address = @event.Address;
            customer.PhoneNumber = @event.PhoneNumber;
            customer.Email = @event.Email;
            
            await _repo.UpdateAsync(customer);
            
            _logger.LogInformation("Customer.Updated event processed successfully for CustomerId: {CustomerId}", @event.CustomerId);
        }

    }

}
