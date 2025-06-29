namespace OrderService.Dto
{
	public class ItemCreateDto
	{
		public Guid ItemId { get; set; }
		public string Name { get; set; }
		public decimal Price { get; set; }
	}
}
