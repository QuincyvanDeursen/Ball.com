using PaymentService.Domain;
using PaymentService.Dto;
using PaymentService.Repository.Interfaces;
using PaymentService.Services.Interfaces;
using Shared.Infrastructure.Messaging.Events.Payments;
using Shared.Infrastructure.Messaging.Interfaces;


namespace PaymentService.Services;
public class PaymentService : IPaymentService
{
    private readonly IPaymentRepo _paymentRepo;
    private readonly IEventPublisher _eventPublisher;


    public PaymentService(
        IPaymentRepo paymentRepo,
        IEventPublisher eventPublisher

       )
    {
        _paymentRepo = paymentRepo;
        _eventPublisher = eventPublisher;
    }


    public async Task<IEnumerable<Payment>> GetAll()
    {
        return await _paymentRepo.GetAllAsync();
    }

    public async Task<Payment> Get(Guid id)
    {
        return await _paymentRepo.GetByIdAsync(id);
    }

    public async Task Create(PaymentCreateDto payment)
    {
        // 1. Create a new payment entity from the DTO
        var newPayment = new Payment
        {
            Id = Guid.NewGuid(),
            TotalPrice = (decimal)payment.TotalPrice,
            Status = PaymentStatus.Pending,
            Customer = payment.Customer,
        };
        // 2. Save the new payment to the database
        await _paymentRepo.CreateAsync(newPayment);

    }

    public async Task Update(Guid id, PaymentUpdateDto payment)
    {
        // 1. Retrieve the old payment and update the status
        var oldPayment = await _paymentRepo.GetByIdAsync(id);
        if (oldPayment == null) throw new KeyNotFoundException($"Payment with ID {id} not found.");

        // 1.1 Update status (pending -> paid | cancelled)
        if (oldPayment.Status is PaymentStatus.Paid or PaymentStatus.Cancelled)
            throw new InvalidOperationException("Payment status cannot be updated after it has been paid or cancelled.");

        oldPayment.Status = payment.Status;

        // 1.2 Update the payment in the database
        await _paymentRepo.UpdateAsync(oldPayment);

        // 2. Publish an event to notify other services about the payment status change
        await PublishPaymentEvent(oldPayment, payment.Status);

    }

    private async Task PublishPaymentEvent(Payment payment, PaymentStatus status)
    {
        if (status == PaymentStatus.Paid)
        {
            var paymentPaidEvent = new PaymentPaidEvent
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
            };
            await _eventPublisher.PublishAsync(paymentPaidEvent);
        }
        else if (status == PaymentStatus.Cancelled)
        {
            var paymentCancelledEvent = new PaymentCancelledEvent
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
            };
            await _eventPublisher.PublishAsync(paymentCancelledEvent);
        }
    }   

}
