using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Infrastructure.Messaging.Interfaces;
using System.Text;

namespace Shared.Infrastructure.Messaging
{
    public class RabbitMqEventConsumer : IEventConsumer, IAsyncDisposable
    {
        private readonly IRabbitMqConnectionFactory _connectionFactory;
        private readonly IEventDispatcher _dispatcher;
        private readonly ILogger<RabbitMqEventConsumer> _logger;
        private readonly string _serviceName;
        private IChannel? _channel;

        public RabbitMqEventConsumer(
            IRabbitMqConnectionFactory connectionFactory,
            IEventDispatcher dispatcher,
            ILogger<RabbitMqEventConsumer> logger,
            string serviceName)
        {
            _connectionFactory = connectionFactory;
            _dispatcher = dispatcher;
            _logger = logger;
            _serviceName = serviceName;
        }

        public async Task ConsumeAsync(CancellationToken cancellationToken)
        {
            _channel = await _connectionFactory.CreateChannelAsync();

            var queueName = $"{_serviceName}.queue";

            await _channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false);
            await _channel.QueueBindAsync(queue: queueName, exchange: "ball.com.exchange", routingKey: "#");

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += OnMessageReceived;

            await _channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);

            _logger.LogInformation("Started consuming events for service {ServiceName}", _serviceName);

            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1000, cancellationToken);
            }
        }

        private async Task OnMessageReceived(object sender, BasicDeliverEventArgs eventArgs)
        {
            var deliveryTag = eventArgs.DeliveryTag;
            try
            {
                var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                var eventType = eventArgs.BasicProperties.Type;

                _logger.LogInformation("Received event {EventType}: {Message}", eventType, message);

                await _dispatcher.DispatchAsync(eventType, message);

                await _channel!.BasicAckAsync(deliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process message. Rejecting it.");
                await _channel!.BasicNackAsync(deliveryTag, false, requeue: false);
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel != null)
            {
                await _channel.CloseAsync();
                _channel.Dispose();
            }
        }
    }
}
