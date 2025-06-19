using PaymentService.Domain;
using System.Data.SqlTypes;

namespace PaymentService.Dto
{
    public class PaymentCreateDto
    {
        public SqlMoney TotalPrice { get; set; }
        public PaymentStatus Status { get; set; }

        public CustomerSnapshot Customer { get; set; }

    }
}
