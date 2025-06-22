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

        public EventStore(AppDbContext context)
        {
            _context = context;
        }

        public async Task SaveAsync(ProductEvent @event)
        {
            var type = @event.GetType().Name;
            var data = JsonSerializer.Serialize(@event, @event.GetType(), _serializerOptions);

            var entity = new EventEntity
            {
                Id = Guid.NewGuid(),
                AggregateId = @event.ProductId,
                EventType = type,
                Data = data,
                Timestamp = @event.Timestamp
            };

            _context.Events.Add(entity);
            await _context.SaveChangesAsync(); // This line requires the Microsoft.EntityFrameworkCore namespace
        }

        public async Task<List<ProductEvent>> GetEventsAsync(Guid productId)
        {
            var events = await _context.Events
                .Where(e => e.AggregateId == productId)
                .OrderBy(e => e.Timestamp)
                .ToListAsync();

            return events.Select(e =>
            {
                var type = Type.GetType($"InventoryManagementService.Events.{e.EventType}")!;
                return (ProductEvent)JsonSerializer.Deserialize(e.Data, type, _serializerOptions)!;
            }).ToList();
        }
    }
}
