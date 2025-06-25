using Microsoft.EntityFrameworkCore;

namespace OrderService.Domain
{
	public class Order
	{
		public Guid OrderId { get; set; }
		public Guid CustomerId { get; set; }
		public CustomerSnapshot Customer { get; set; }
		public DateTime OrderDate { get; set; }
		public decimal TotalPrice { get; set; }
		public PaymentStatus PaymentStatus { get; set; }
		public OrderStatus OrderStatus { get; set; }
		public List<OrderItem> OrderItems { get; set; } = new();
		
	}

	[Owned]
	public class CustomerSnapshot
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string PhoneNumber { get; set; }

		public string Address { get; set; }
		public string Email { get; set; }

	}
}
