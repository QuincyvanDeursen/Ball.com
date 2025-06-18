using Shared.Infrastructure.Messaging;

namespace PaymentService.Services
{
    public class RabbitMqBackgroundService : BackgroundService, IAsyncDisposable
    {
        private readonly RabbitMqEventConsumer _consumer;

        public RabbitMqBackgroundService(RabbitMqEventConsumer consumer)
        {
            _consumer = consumer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _consumer.ConsumeAsync(stoppingToken);
        }

        public async ValueTask DisposeAsync()
        {
            await _consumer.DisposeAsync();
        }
    }
}
