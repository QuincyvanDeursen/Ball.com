using InventoryManagementService.Data;
using InventoryManagementService.Events;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace InventoryManagementService.Repositories
{
    public class EventStore : IEventStore
    {
        private readonly AppDbContext _context;
        private readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        private readonly ILogger<EventStore> _logger;

        public EventStore(AppDbContext context, ILogger<EventStore> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SaveAsync(ItemDomainEvent @event)
        {
            


            var type = @event.GetType().Name;
            var data = JsonSerializer.Serialize(@event, @event.GetType(), _serializerOptions);

            var entity = new EventEntity
            {
                Id = Guid.NewGuid(),
                AggregateId = @event.ItemId,
                EventType = type,
                Data = data,
                Timestamp = @event.Timestamp
            };

            _context.Events.Add(entity);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Saved event of type {EventType} for ItemId {ItemId} to the eventstore", @event.GetType().Name, @event.ItemId);
        }

        public async Task<List<ItemDomainEvent>> GetEventsAsync(Guid productId)
        {
            _logger.LogInformation("Retrieving all events from the eventstore for ItemId {ItemId}", productId);
            var events = await _context.Events
                .Where(e => e.AggregateId == productId)
                .OrderBy(e => e.Timestamp)
                .ToListAsync();

            return events.Select(e =>
            {
                var type = Type.GetType($"InventoryManagementService.Events.{e.EventType}")!;
                return (ItemDomainEvent)JsonSerializer.Deserialize(e.Data, type, _serializerOptions)!;
            }).ToList();
        }
    }
}
