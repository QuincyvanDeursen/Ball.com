using PaymentService.Domain;
using PaymentService.Repository.Interfaces;
using Shared.Infrastructure.Messaging.Events.Interfaces;
using Shared.Infrastructure.Messaging.Events.Orders;

namespace PaymentService.Handlers
{
    public class PaymentOrderCancelledHandler : IEventHandler<OrderCancelledEvent>
    {
        private readonly ILogger<PaymentOrderCancelledHandler> _logger;
        private readonly IPaymentRepo _repo;
        public PaymentOrderCancelledHandler(ILogger<PaymentOrderCancelledHandler> logger, IPaymentRepo paymentRepo)
        {
            _logger = logger;
            _repo = paymentRepo;
        }
        public async Task HandleAsync(OrderCancelledEvent @event)
        {
            _logger.LogInformation("Handling OrderCancelled event for OrderId: {OrderId}", @event.OrderId);

            // Check if payment already exists for this order
            Payment existingPayment;
            try
            {
                existingPayment = await _repo.GetByOrderId(@event.OrderId);
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("No payment found for OrderId: {OrderId}", @event.OrderId);
                return;
            }

            existingPayment.Status = PaymentStatus.Cancelled;

            await _repo.UpdateAsync(existingPayment);

            _logger.LogInformation("Payment cancelled successfully for OrderId: {OrderId}", @event.OrderId);

        }
    }
}
