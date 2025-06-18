using PaymentService.Domain;
using PaymentService.Dto;

namespace PaymentService.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<IEnumerable<Payment>> GetAll();
        Task<Payment> Get(Guid id);
        Task Create(PaymentCreateDto payment);
        Task Update(Guid id, PaymentUpdateDto payment);
    }
}
