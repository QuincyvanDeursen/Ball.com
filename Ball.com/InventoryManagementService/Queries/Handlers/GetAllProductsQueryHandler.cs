using InventoryManagementService.Data;
using InventoryManagementService.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementService.Queries.Handlers
{
    public class GetAllProductsQueryHandler : IQueryHandler<GetAllProductsQuery, IEnumerable<ProductReadModel>>
    {
        private readonly AppDbContext _context;

        public GetAllProductsQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductReadModel>> HandleAsync(GetAllProductsQuery query)
        {
            return await _context.ProductReadModels.ToListAsync();
        }
    }
}
