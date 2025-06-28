using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Infrastructure.Messaging.Events.Orders
{
	public class OrderValidatedEvent : BaseEvent
	{
		public override string EventType => "order.validated";

		public Guid OrderId { get; set; }
		public Guid CustomerId { get; set; }
		public DateTime OrderDate { get; set; }
		public decimal TotalPrice { get; set; }

		public string FirstName { get; set; } = null!;
		public string LastName { get; set; } = null!;
		public string Email { get; set; } = null!;
		public string Address { get; set; } = null!;
		public string PhoneNumber { get; set; } = null!;

		public List<OrderItemDto> Items { get; set; } = new();
	}
}
