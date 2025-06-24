namespace Shared.Infrastructure.Messaging.Events.Orders
{

    public class OrderItemDto
    {
        public Guid ItemId { get; set; }
        public string Name { get; set; } = null!;  // optioneel
        public int OrderQuantity { get; set; }
        public decimal Price { get; set; }
    }
}
