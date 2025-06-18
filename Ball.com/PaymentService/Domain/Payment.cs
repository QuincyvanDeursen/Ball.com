using System.ComponentModel.DataAnnotations;

namespace PaymentService.Domain
{
    public class Payment
    {
        public Guid Id { get; init; }
        public decimal TotalPrice { get; set; }
        public PaymentStatus Status { get; set; }
        public PaymentCustomer Customer { get; set; }
    }
}
