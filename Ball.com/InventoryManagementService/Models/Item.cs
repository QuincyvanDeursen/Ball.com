using InventoryManagementService.Events;

namespace InventoryManagementService.Models
{
    public class Item
    {
        public Guid Id { get;  set; }
        public string Name { get;  set; } = null!;
        public string Description { get;  set; } = null!;
        public decimal Price { get;  set; }
        public int Stock { get;  set; }

        public void Apply(ItemDomainEvent @event)
        {
            switch (@event)
            {
                case ItemCreatedDomainEvent e:
                    Id = e.ItemId;
                    Name = e.Name;
                    Description = e.Description;
                    Price = e.Price;
                    Stock = e.Stock;
                    break;

                case StockUpdatedDomainEvent e:
                    Stock += e.Amount;
                    break;

                    // Voeg hier andere events toe
            }
        }
    }
}