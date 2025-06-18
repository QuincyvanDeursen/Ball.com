using PaymentService.Domain;

namespace PaymentService.Repository.Interfaces
{
    public interface IPaymentRepo
    {
        Task<Payment> GetByIdAsync(Guid id);
        Task<IEnumerable<Payment>> GetAllAsync();

        Task CreateAsync (Payment payment); 
        Task UpdateAsync (Payment payment);

        Task<IEnumerable<Payment>> GetByCustomerId(Guid customerId);
    }
}
