namespace Shared.Infrastructure.Messaging.Events.Payments
{
    public class PaymentPaidEvent : BaseEvent
    {
        public override string EventType => "payment.paid";
        public Guid PaymentId { get; set; }
        public Guid OrderId { get; set; }

    }
}
