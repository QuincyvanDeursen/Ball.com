using InventoryManagementService.Models;

namespace InventoryManagementService.Queries
{
    public class GetProductByIdQuery : IQuery<ProductReadModel>
    {
        public Guid Id { get; set; }
    }
}
