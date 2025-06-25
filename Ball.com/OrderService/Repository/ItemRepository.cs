
using Microsoft.EntityFrameworkCore;
using OrderService.Database;
using OrderService.Domain;
using OrderService.Repository.Interfaces;

namespace ItemService.Repository
{
	public class ItemRepository : IItemRepository
	{
		private readonly OrderDbContext _context;

		public ItemRepository(OrderDbContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public async Task CreateAsync(Item item)
		{
			if (item == null)
			{
				throw new ArgumentNullException(nameof(item));
			}
			await _context.Items.AddAsync(item);
			await _context.SaveChangesAsync();
		}

		public async Task<Item> GetByIdAsync(Guid id)
		{
			if (id == Guid.Empty)
			{
				throw new ArgumentException("Invalid order ID.", nameof(id));
			}
			return await _context.Items.FirstOrDefaultAsync(o => o.ItemId == id) ?? throw new KeyNotFoundException($"item with ID {id} not found.");
		}

		public async Task UpdateAsync(Item item)
		{
			if (item == null)
			{
				throw new ArgumentNullException(nameof(item));
			}
			_context.Items.Update(item);
			await _context.SaveChangesAsync();
		}
	}
}
