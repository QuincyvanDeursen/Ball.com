using InventoryManagementService.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementService.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<EventEntity> Events { get; set; }
        public DbSet<ProductReadModel> ProductReadModels { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}
