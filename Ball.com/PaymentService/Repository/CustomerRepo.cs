using Microsoft.EntityFrameworkCore;
using PaymentService.Database;
using PaymentService.Domain;
using PaymentService.Repository.Interfaces;

namespace PaymentService.Repository
{
    public class CustomerRepo : ICustomerRepo
    {
        private readonly PaymentDbContext _context;

        public CustomerRepo(PaymentDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task CreateAsync(Customer customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            
            return await _context.Customers.ToListAsync();
        }

        public async Task<Customer> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Invalid customer ID.", nameof(id));
            }
            return await _context.Customers.FindAsync(id) ?? throw new KeyNotFoundException($"Customer with ID {id} not found.");
        }

        public Task UpdateAsync(Customer customer)
        {   
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer));
            }
            _context.Customers.Update(customer);
            return _context.SaveChangesAsync();
        }
    }
}
