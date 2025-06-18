using Microsoft.EntityFrameworkCore;
using PaymentService.Database;
using PaymentService.Domain;
using PaymentService.Repository.Interfaces;

namespace PaymentService.Repository
{
    public class PaymentRepo(PaymentDbContext context) : IPaymentRepo
    {
        public async Task CreateAsync(Payment payment)
        {
            await context.Payments.AddAsync(payment);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            return await context.Payments
              .Include(p => p.Customer)
              .ToListAsync();
        }

        public async Task<Payment> GetByIdAsync(Guid id)
        {
            var payment = await context.Payments
                .Include(p => p.Customer)
                .FirstOrDefaultAsync(p => p.Id == id);

            return payment is null ? throw new InvalidOperationException($"Payment with id '{id}' was not found.") : payment;
        }

        public async Task UpdateAsync(Payment payment)
        {
            context.Payments.Update(payment);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Payment>> GetAllByCustomerId(Guid customerId)
        {
            return await context.Payments
                .Where(p => p.Customer.Id == customerId)
                .ToListAsync();
        }
    }
}
