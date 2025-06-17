using CustomerService.Domain;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Database
{
    public class CustomerDbContext : DbContext
    {

        public DbSet<Customer> Customers { get; set; }

        public CustomerDbContext(DbContextOptions<CustomerDbContext> options) : base(options)
        {
        }
    }
}
