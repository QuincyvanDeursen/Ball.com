namespace OrderService.Dto
{
	public class ItemUpdateDto
	{
		public Guid ItemId { get; set; }
		public string? Name { get; set; }
		public decimal? Price { get; set; }
	}
}
