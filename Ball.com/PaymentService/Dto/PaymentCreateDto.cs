using PaymentService.Domain;

namespace PaymentService.Dto
{
    public class PaymentCreateDto
    {
        public decimal TotalPrice { get; set; }
        public PaymentStatus Status { get; set; }

        public Customer Customer { get; set; }

    }
}
