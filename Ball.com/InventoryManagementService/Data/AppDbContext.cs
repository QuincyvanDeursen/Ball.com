using InventoryManagementService.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementService.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<EventEntity> Events { get; set; }
        public DbSet<ItemReadModel> ItemReadModels { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}
