using Microsoft.Extensions.Hosting;
using Shared.Infrastructure.Messaging.Interfaces;

namespace  Shared.Infrastructure.Messaging.BackGroundService
{
    public class RabbitMqBackgroundService : BackgroundService, IAsyncDisposable
    {
        private readonly IEventConsumer _consumer;

        public RabbitMqBackgroundService(IEventConsumer consumer)
        {
            _consumer = consumer ?? throw new ArgumentNullException(nameof(consumer), "Consumer cannot be null. Ensure it is registered in the service collection.");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _consumer.ConsumeAsync(stoppingToken);
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            // Fix: Ensure the consumer implements IDisposable or IAsyncDisposable
            if (_consumer is IAsyncDisposable asyncDisposableConsumer)
            {
                await asyncDisposableConsumer.DisposeAsync();
            }
            else if (_consumer is IDisposable disposableConsumer)
            {
                disposableConsumer.Dispose();
            }
            // Fix: Call GC.SuppressFinalize to prevent derived types with finalizers from needing to re-implement IDisposable
            GC.SuppressFinalize(this);
        }
    }
}
