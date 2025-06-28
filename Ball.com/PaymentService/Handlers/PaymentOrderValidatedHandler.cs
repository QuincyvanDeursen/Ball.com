using PaymentService.Domain;
using PaymentService.Repository.Interfaces;
using Shared.Infrastructure.Messaging.Events.Interfaces;
using Shared.Infrastructure.Messaging.Events.Orders;

namespace PaymentService.Handlers
{
    public class PaymentOrderValidatedHandler : IEventHandler<OrderValidatedEvent>
    {
        private readonly IPaymentRepo _repo;
        private readonly ILogger<PaymentOrderValidatedHandler> _logger;

        public PaymentOrderValidatedHandler(IPaymentRepo repo, ILogger<PaymentOrderValidatedHandler> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task HandleAsync(OrderValidatedEvent @event)
        {
            _logger.LogInformation("Handling OrderValidated event for OrderId: {OrderId}", @event.OrderId);

            // Check if payment already exists for this order
            Payment existingPayment;
            try
            {
                existingPayment = await _repo.GetByOrderId(@event.OrderId);
                _logger.LogInformation("Payment for OrderId {OrderId} already exists. Skipping creation.", @event.OrderId);
                return;
            }
            catch (KeyNotFoundException)
            {
                // Payment does not exist, continue to create
            }

            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                OrderId = @event.OrderId,
                TotalPrice = @event.TotalPrice,
                Status = PaymentStatus.Pending,
                Customer = new CustomerSnapshot
                {
                    Id = @event.CustomerId,
                    FirstName = @event.FirstName,
                    LastName = @event.LastName,
                    Address = @event.Address,
                    PhoneNumber = @event.PhoneNumber,
                    Email = @event.Email
                }
            };

            await _repo.CreateAsync(payment);

            _logger.LogInformation("Payment created successfully for OrderId: {OrderId}", @event.OrderId);
        }
    }
}
