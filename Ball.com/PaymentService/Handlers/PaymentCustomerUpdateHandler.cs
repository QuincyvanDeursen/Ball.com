using PaymentService.Repository.Interfaces;
using Shared.Infrastructure.Messaging.Events;
using Shared.Infrastructure.Messaging.Events.Interfaces;

namespace PaymentService.Handlers
{
    public class PaymentCustomerUpdatedHandler : IEventHandler<CustomerUpdatedEvent>
    {
        // Inject eventueel repositories of services die je nodig hebt
        private readonly IPaymentRepo _paymentRepository;

        public PaymentCustomerUpdatedHandler(IPaymentRepo paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task HandleAsync(CustomerUpdatedEvent @event)
        {
            // Haal alle payments op voor deze klant
            var payments = await _paymentRepository.GetAllByCustomerId(@event.CustomerId);

            if (payments != null && payments.Any())
            {

                foreach (var payment in payments)
                {
                    payment.Customer.PhoneNumber = @event.PhoneNumber;
                    payment.Customer.Email = @event.Email;
                    await _paymentRepository.UpdateAsync(payment);
                }
            }
        }

    }

}
