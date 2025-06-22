using InventoryManagementService.Data;
using InventoryManagementService.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementService.Queries.Handlers
{
    public class GetProductByIdQueryHandler : IQueryHandler<GetProductByIdQuery, ProductReadModel?>
    {
        private readonly AppDbContext _context;

        public GetProductByIdQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProductReadModel?> HandleAsync(GetProductByIdQuery query)
        {
            return await _context.ProductReadModels
                .FirstOrDefaultAsync(p => p.Id == query.Id);
        }
    }
}
