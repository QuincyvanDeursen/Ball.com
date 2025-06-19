using PaymentService.Domain;

namespace PaymentService.Repository.Interfaces
{
    public interface ICustomerRepo
    {
   
            Task<Customer> GetByIdAsync(Guid id);
            Task<IEnumerable<Customer>> GetAllAsync();
            Task CreateAsync(Customer customer);
            Task UpdateAsync(Customer customer);
        
    }
}
