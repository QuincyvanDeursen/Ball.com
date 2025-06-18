using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Shared.Infrastructure.Messaging.Events.Interfaces;
using Shared.Infrastructure.Messaging.Interfaces;
using System.Text;
using System.Text.Json;

namespace Shared.Infrastructure.Messaging
{
    public class RabbitMqEventPublisher : IEventPublisher
    {
        private readonly IRabbitMqConnectionFactory _connectionFactory;
        private readonly ILogger<RabbitMqEventPublisher> _logger;

        public RabbitMqEventPublisher(
            IRabbitMqConnectionFactory connectionFactory,
            ILogger<RabbitMqEventPublisher> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public async Task PublishAsync<T>(T @event) where T : IEvent
        {
            await PublishAsync(@event, @event.EventType);
        }

        public async Task PublishAsync<T>(T @event, string routingKey) where T : IEvent
        {
            using var channel = await _connectionFactory.CreateChannelAsync();

            // Voeg handler toe voor returned messages (niet bezorgd)
            channel.BasicReturnAsync += async (sender, args) =>
            {
                try
                {
                    var returnedMessage = Encoding.UTF8.GetString(args.Body.ToArray());
                    var reason = args.ReplyText;
                    var rk = args.RoutingKey;
                    _logger.LogWarning("Message returned by broker. Reason: {Reason}, RoutingKey: {RoutingKey}, Message: {Message}",
                        reason, rk, returnedMessage);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error handling returned message.");
                }
                await Task.CompletedTask; // Ensure all code paths return a Task.
            };

            var message = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(message);

            var properties = new BasicProperties
            {
                Persistent = true,
                MessageId = @event.Id.ToString(),
                Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
                Type = @event.EventType
            };

            await channel.BasicPublishAsync(
                exchange: "ball.com.exchange", // Je exchange naam
                routingKey: routingKey,
                mandatory: true,  // Nu true
                basicProperties: properties,
                body: body);

            _logger.LogInformation("Published event {EventType} with Event ID {EventId}",
                @event.EventType, @event.Id);
        }
    }
}
