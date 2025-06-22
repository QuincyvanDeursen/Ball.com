using PaymentService.Domain;
using PaymentService.Repository.Interfaces;
using Shared.Infrastructure.Messaging.Events;
using Shared.Infrastructure.Messaging.Events.Interfaces;

namespace PaymentService.Handlers
{
    public class PaymentCustomerCreatedHandler : IEventHandler<CustomerCreatedEvent>
    {

        // Deze handler luistert naar het CustomerCreatedEvent en maakt een nieuwe klant aan in de PaymentService database.
        // De reden dat we dit doen is omdat paymentService geen directe database connectie heeft met de CustomerService.
        // We slaan dus een kopie van de klantgegevens op in de PaymentService database. De Payment records zelf, bevatten de informatie over de klant, zoals telefoonnummer, naam, adres en email.
        // Dat is echt een een historisch feit en mag niet aangepast worden, omdat het anders de integriteit van de data zou kunnen aantasten.
        // Mocht de klant zijn gegevens willen aanpassen, dan zal de CustomerService een CustomerUpdatedEvent sturen.
        // De payment records mogen dus NIET worden aangepast, maar de payment moet wel herleid worden naar de nieuwe klantgegevens.

        private readonly ICustomerRepo _repo;
        private readonly ILogger<PaymentCustomerCreatedHandler> _logger;

        public PaymentCustomerCreatedHandler(ICustomerRepo repo, ILogger<PaymentCustomerCreatedHandler> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task HandleAsync(CustomerCreatedEvent @event)
        {
            _logger.LogInformation("Handling Customer.Created event for CustomerId: {CustomerId}", @event.CustomerId);

            Customer customer;

            try
            {
                customer = await _repo.GetByIdAsync(@event.CustomerId);

                // Optional: Als de klant al bestaat, hoef je hem niet opnieuw aan te maken
                _logger.LogInformation("Customer with ID {CustomerId} already exists. Skipping creation.", @event.CustomerId);
                return;
            }
            catch (KeyNotFoundException)
            {
                customer = new Customer
                {
                    Id = @event.CustomerId,
                    FirstName = @event.FirstName,
                    LastName = @event.LastName,
                    Address = @event.Address,
                    PhoneNumber = @event.PhoneNumber,
                    Email = @event.Email
                };
            }

            await _repo.CreateAsync(customer);

            _logger.LogInformation("Customer.Created event processed successfully for CustomerId: {CustomerId}", @event.CustomerId);
        }

    }
}
