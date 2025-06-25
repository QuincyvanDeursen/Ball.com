using Microsoft.EntityFrameworkCore;
using OrderService.Database;
using OrderService.Domain;
using OrderService.Repository.Interfaces;

namespace OrderService.Repository
{
	public class CustomerRepository : ICustomerRepository
	{
		private readonly OrderDbContext _context;

		public CustomerRepository(OrderDbContext context)
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

		public async Task<Customer> GetByIdAsync(Guid id)
		{
			if (id == Guid.Empty)
			{
				throw new ArgumentException("Invalid order ID.", nameof(id));
			}
			return await _context.Customers.FirstOrDefaultAsync(o => o.CustomerId == id) ?? throw new KeyNotFoundException($"customer with ID {id} not found.");

		}

		public async Task UpdateAsync(Customer customer)
		{
			if (customer == null)
			{
				throw new ArgumentNullException(nameof(customer));
			}
			_context.Customers.Update(customer);
			await _context.SaveChangesAsync();
		}
	}
}
