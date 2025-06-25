using Microsoft.EntityFrameworkCore;
using OrderService.Domain;

namespace OrderService.Database
{
	public class OrderDbContext : DbContext
	{
		public DbSet<Customer> Customers { get; set; }
		public DbSet<Item> Items { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderItem> OrderItems { get; set; }
		public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Order>().OwnsOne(p => p.Customer);
			// Configure the composite primary key for OrderItem
			modelBuilder.Entity<OrderItem>()
				.HasKey(oi => new { oi.ItemId, oi.OrderId, oi.Id });
			modelBuilder.Entity<Order>()
			.HasMany(o => o.OrderItems)
			.WithOne(oi => oi.Order)
			.HasForeignKey(oi => oi.OrderId);
		}
	}
}
