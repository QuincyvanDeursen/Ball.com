using InventoryManagementService.Data;
using InventoryManagementService.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementService.Services
{
    public class EventReplayer : IEventReplayer
    {
        private readonly AppDbContext _context;
        private readonly IEventStore _eventStore;
        private readonly IReadModelUpdater _updater;

        public EventReplayer(AppDbContext context, IEventStore eventStore, IReadModelUpdater updater)
        {
            _context = context;
            _eventStore = eventStore;
            _updater = updater;
        }

        public async Task ReplayAsync()
        {
            // 1. Wis de bestaande readmodels
            _context.ItemReadModels.RemoveRange(_context.ItemReadModels);
            await _context.SaveChangesAsync();

            // 2. Vind unieke aggregate roots (Product IDs)
            var aggregateIds = await _context.Events
                .Select(e => e.AggregateId)
                .Distinct()
                .ToListAsync();

            // 3. Reconstructie per aggregate
            foreach (var aggregateId in aggregateIds)
            {
                var events = await _eventStore.GetEventsAsync(aggregateId);

                foreach (var @event in events.OrderBy(e => e.Timestamp))
                {
                    await _updater.ApplyAsync(@event);
                }
            }
        }
    }
}
