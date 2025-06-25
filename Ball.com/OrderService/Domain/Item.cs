namespace OrderService.Domain
{
	public class Item
	{
		public Guid ItemId { get; set; }
		public string Name { get; set; }
		public decimal Price { get; set; }
		public int Stock { get; set; }
	}
}
