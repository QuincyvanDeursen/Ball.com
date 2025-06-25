using OrderService.Domain;

namespace OrderService.Dto
{
	public class OrderCreateDto
	{
		public Guid CustomerId { get; set; }
		public List<OrderItemCreateDto> OrderItems { get; set; } = new();

	}

	public class OrderItemCreateDto
	{
		public Guid ItemId { get; set; }
		public int Quantity { get; set; }
	}
}
