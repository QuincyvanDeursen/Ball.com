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
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
            _updater = updater ?? throw new ArgumentNullException(nameof(updater));
        }

        public async Task ReplayAsync()
        {
            // 1. Leeg de bestaande read models
            _context.ItemReadModels.RemoveRange(_context.ItemReadModels);
            await _context.SaveChangesAsync();

            // 2. Haal unieke aggregateIds op
            var aggregateIds = await _context.Events
                .Select(e => e.AggregateId)
                .Distinct()
                .ToListAsync();

            // 3. Haal events op per aggregate en bouw + sla het readmodel op
            foreach (var aggregateId in aggregateIds)
            {
                var events = await _eventStore.GetEventsAsync(aggregateId);
                await _updater.RestoreAndSave(events);
            }
        }
    }
}
