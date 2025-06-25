using Shared.Infrastructure.Messaging;
using Shared.Infrastructure.Messaging.Interfaces;

namespace OrderService.Services
{
    public class RabbitMqBackgroundService : BackgroundService, IAsyncDisposable
    {
        private readonly IEventConsumer _consumer;

        public RabbitMqBackgroundService(IEventConsumer consumer)
        {
            _consumer = consumer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _consumer.ConsumeAsync(stoppingToken);
        }

        public async ValueTask DisposeAsync()
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
        }
    }
}
