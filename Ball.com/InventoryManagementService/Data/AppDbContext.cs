using InventoryManagementService.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementService.Data
{
    public class AppDbContext : DbContext
    {
        //This is for event sourcing, where we store events that represent state changes in the system.
        public DbSet<EventEntity> Events { get; set; }

        //This is for the read model, which is a denormalized view of the data optimized for querying.
        public DbSet<ItemReadModel> ItemReadModels { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}
