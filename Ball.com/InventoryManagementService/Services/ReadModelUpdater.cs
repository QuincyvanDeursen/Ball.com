using InventoryManagementService.Data;
using InventoryManagementService.Events;
using InventoryManagementService.Models;

namespace InventoryManagementService.Services
{
    public class ReadModelUpdater : IReadModelUpdater
    {
        private readonly AppDbContext _context;

        public ReadModelUpdater(AppDbContext context)
        {
            _context = context;
        }

        public async Task ApplyAsync(ProductEvent @event)
        {
            switch (@event)
            {
                case ProductCreated e:
                    _context.ProductReadModels.Add(new ProductReadModel
                    {
                        Id = e.ProductId,
                        Name = e.Name,
                        Description = e.Description,
                        Price = e.Price,
                        Stock = e.InitialStock
                    });
                    break;

                case StockUpdatedEvent e:
                    var product = await _context.ProductReadModels.FindAsync(e.ProductId);
                    if (product != null) product.Stock += e.Amount;
                    break;
            }

            await _context.SaveChangesAsync();
        }
    }
}
