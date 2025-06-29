namespace CustomerService.Dto
{
    public class CustomerUpdateDto
    {
        public Guid Id { get; set; }
        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }
    }
}
