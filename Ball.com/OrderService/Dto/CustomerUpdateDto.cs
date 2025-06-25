namespace OrderService.Dto
{
	public class CustomerUpdateDto
	{
		public Guid CustomerId { get; set; }
		public string PhoneNumber { get; set; } = string.Empty;

		public string Address { get; set; } = string.Empty;

		public string Email { get; set; } = string.Empty;
	}
}
