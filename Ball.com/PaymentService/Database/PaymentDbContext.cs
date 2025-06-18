using Microsoft.EntityFrameworkCore;
using PaymentService.Domain;

namespace PaymentService.Database
{
    public class PaymentDbContext : DbContext
    {
        public DbSet<Payment> Payments { get; set; }

        public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Payment>().OwnsOne(p => p.Customer);
        }
    }
}
