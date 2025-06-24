using PaymentService.Domain;
using PaymentService.Dto;

namespace PaymentService.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<Payment>> GetAll();
        Task<Payment> Get(Guid id);
    }
}
