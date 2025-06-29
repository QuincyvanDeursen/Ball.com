using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Infrastructure.Messaging.Events;
using Shared.Infrastructure.Messaging.Events.Interfaces;
using Shared.Infrastructure.Messaging.Events.Items;
using Shared.Infrastructure.Messaging.Events.Orders;
using Shared.Infrastructure.Messaging.Events.Payments;
using Shared.Infrastructure.Messaging.Interfaces;
using System.Text.Json;

namespace Shared.Infrastructure.Messaging
{
    public class EventDispatcher : IEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EventDispatcher> _logger;

        // Mappen eventType strings naar CLR event types
        private readonly Dictionary<string, Type> _eventTypes = new()
        {
            { "customer.updated", typeof(CustomerUpdatedEvent) },
            { "customer.created", typeof(CustomerCreatedEvent) },
            { "payment.paid", typeof(PaymentPaidEvent) },
            { "payment.cancelled", typeof(PaymentCancelledEvent) },
            { "order.placed", typeof(OrderPlacedEvent) },
            { "order.cancelled", typeof(OrderCancelledEvent) },
            { "item.created", typeof(ItemCreatedEvent) },
            { "item.updated", typeof(ItemUpdatedEvent) },
            { "stock.updated", typeof(StockUpdatedEvent) },
            {"order.validated", typeof(OrderValidatedEvent) }
            // Voeg hier je andere event mappings toe
        };

        public EventDispatcher(IServiceProvider serviceProvider, ILogger<EventDispatcher> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task DispatchAsync(string eventType, string message)
        {
            using var scope = _serviceProvider.CreateScope();

            if (!_eventTypes.TryGetValue(eventType, out var eventClrType))
            {
                _logger.LogWarning("Unknown event type received: {EventType}", eventType);
                return;
            }

            var @event = JsonSerializer.Deserialize(message, eventClrType);
            if (@event == null) return;

            var handlerType = typeof(IEventHandler<>).MakeGenericType(eventClrType);
            var handlers = scope.ServiceProvider.GetServices(handlerType).ToList();

            if (handlers.Count == 0)
            {
                _logger.LogInformation("Event received but no handlers registered for: {EventType}", eventType);
                return;
            }

            foreach (var handler in handlers)
            {
                var handleMethod = handlerType.GetMethod("HandleAsync");
                if (handleMethod == null) continue;

                var task = (Task)handleMethod.Invoke(handler, new[] { @event })!;
                await task;
            }
        }
    }
}
