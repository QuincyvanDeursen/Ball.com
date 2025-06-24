using InventoryManagementService.Data;
using InventoryManagementService.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementService.Queries.Handlers
{
    public class GetAllItemsQueryHandler : IQueryHandler<GetAllItemsQuery, IEnumerable<ItemReadModel>>
    {
        private readonly AppDbContext _context;

        public GetAllItemsQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ItemReadModel>> HandleAsync(GetAllItemsQuery query)
        {
            return await _context.ProductReadModels.ToListAsync();
        }
    }
}
