using OrderService.Domain;

namespace OrderService.Dto
{
	public class OrderUpdateDto
	{
		public Guid OrderId { get; set; }
		public PaymentStatus? PaymentStatus { get; set; }
		public OrderStatus? OrderStatus { get; set; }
	}
}
