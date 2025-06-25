using Microsoft.EntityFrameworkCore;
using OrderService.Database;
using OrderService.Domain;
using OrderService.Repository.Interfaces;

namespace OrderService.Repository
{
	public class OrderRepository : IOrderRepository
	{
		private readonly OrderDbContext _context;

		public OrderRepository(OrderDbContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}
		public async Task CreateAsync(Order order)
		{
			if (order == null)
			{
				throw new ArgumentNullException(nameof(order));
			}
			await _context.Orders.AddAsync(order);
			await _context.SaveChangesAsync();
		}

		public async Task<IEnumerable<Order>> GetAllAsync()
		{
			return await _context.Orders
				.Include(o => o.OrderItems)
				.ToListAsync();
		}

		public async Task<Order> GetByIdAsync(Guid id)
		{
			if (id == Guid.Empty)
			{
				throw new ArgumentException("Invalid order ID.", nameof(id));
			}
			return await _context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.OrderId == id) ?? throw new KeyNotFoundException($"order with ID {id} not found.");
		}

		public async Task UpdateAsync(Order order)
		{
			if (order == null)
			{
				throw new ArgumentNullException(nameof(order));
			}
			_context.Orders.Update(order);
			await _context.SaveChangesAsync();
		}
	}
}
