namespace Shared.Infrastructure.Messaging.Events
{
    public class CustomerUpdatedEvent : BaseEvent
    {
        public override string EventType => "customer.updated";

        public Guid CustomerId { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

    }
}
