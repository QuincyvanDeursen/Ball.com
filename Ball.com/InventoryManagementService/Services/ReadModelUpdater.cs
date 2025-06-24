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

        public async Task ApplyAsync(ItemDomainEvent @event)
        {
            switch (@event)
            {
                case ItemCreatedDomainEvent e:
                    _context.ProductReadModels.Add(new ItemReadModel
                    {
                        Id = e.ItemId,
                        Name = e.Name,
                        Description = e.Description,
                        Price = e.Price,
                        Stock = e.Stock
                    });
                    break;

                case StockUpdatedDomainEvent e:
                    var product = await _context.ProductReadModels.FindAsync(e.ItemId);
                    if (product != null) product.Stock += e.Amount;
                    break;
                case ItemUpdatedDomainEvent e:
                    var updatedProduct = await _context.ProductReadModels.FindAsync(e.ItemId);
                    if (updatedProduct != null)
                    {
                        updatedProduct.Name = e.Name;
                        updatedProduct.Description = e.Description;
                        updatedProduct.Price = e.Price;
                    }
                    break;
            }

            await _context.SaveChangesAsync();
        }
    }
}
