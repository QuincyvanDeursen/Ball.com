using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Infrastructure.Messaging.Events.Payments
{
    public class PaymentCancelledEvent : BaseEvent
    {
        public override string EventType => "payment.cancelled";

        public Guid PaymentId { get; set; }
        public Guid OrderId { get; set; }
    }
}
