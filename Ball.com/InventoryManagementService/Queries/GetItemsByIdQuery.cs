using InventoryManagementService.Models;

namespace InventoryManagementService.Queries
{
    public class GetItemsByIdQuery : IQuery<ItemReadModel>
    {
        public Guid Id { get; set; }
    }
}
