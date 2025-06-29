using InventoryManagementService.Data;
using InventoryManagementService.Events;
using InventoryManagementService.Models;

namespace InventoryManagementService.Services
{
    public class ReadModelUpdater : IReadModelUpdater
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ReadModelUpdater> _logger;

        public ReadModelUpdater(AppDbContext context, ILogger<ReadModelUpdater> logger)
        {
            _context = context;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task ApplyAsync(ItemDomainEvent @event)
        {
            ItemReadModel? readModel;

            if (@event is ItemCreatedDomainEvent)
            {
                // Nieuw model opbouwen
                readModel = new ItemReadModel();
                readModel.Apply(@event);
                _context.ItemReadModels.Add(readModel);
            }
            else
            {
                // Bestaand model ophalen en bijwerken
                readModel = await _context.ItemReadModels.FindAsync(@event.ItemId);
                if (readModel == null)
                {
                    throw new InvalidOperationException($"Read model not found for item with ID '{@event.ItemId}'");
                }

                readModel.Apply(@event);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<ItemReadModel?> RestoreAndSave(IEnumerable<ItemDomainEvent> events)
        {
            var readModel = new ItemReadModel();

            foreach (var @event in events.OrderByDescending(e => e.Timestamp))
            {
                _logger.LogInformation("Applying event of type {EventType} with ID {EventId} to read model", @event.GetType().Name, @event.ItemId);
                readModel.Apply(@event);
            }

            if (readModel.Id == Guid.Empty)
            {
            
                return null;
            }

            _context.ItemReadModels.Add(readModel);
            await _context.SaveChangesAsync();

            return readModel;
        }
    }
}
