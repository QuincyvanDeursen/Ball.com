using InventoryManagementService.Data;
using InventoryManagementService.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementService.Queries.Handlers
{
    public class GetItemsByIdQueryHandler : IQueryHandler<GetItemsByIdQuery, ItemReadModel?>
    {
        private readonly AppDbContext _context;

        public GetItemsByIdQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ItemReadModel?> HandleAsync(GetItemsByIdQuery query)
        {
            return await _context.ItemReadModels
                .FirstOrDefaultAsync(p => p.Id == query.Id);
        }
    }
}
