using InventoryManagementService.Events;

namespace InventoryManagementService.Models
{
    public class Product
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = null!;
        public string Description { get; private set; } = null!;
        public decimal Price { get; private set; }
        public int Stock { get; private set; }

        public void Apply(ProductEvent @event)
        {
            switch (@event)
            {
                case ProductCreated e:
                    Id = e.ProductId;
                    Name = e.Name;
                    Description = e.Description;
                    Price = e.Price;
                    Stock = e.InitialStock;
                    break;

                case StockUpdatedEvent e:
                    Stock += e.Amount;
                    break;

                    // Voeg hier andere events toe
            }
        }
    }
}