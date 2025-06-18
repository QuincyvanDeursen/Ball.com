using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Shared.Infrastructure.Messaging.Configuration;
using Shared.Infrastructure.Messaging.Interfaces;

namespace Shared.Infrastructure.Messaging
{
    public class RabbitMqConnectionFactory : IRabbitMqConnectionFactory, IAsyncDisposable
    {
        private readonly RabbitMqSettings _settings;
        private IConnection? _connection;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public RabbitMqConnectionFactory(IOptions<RabbitMqSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task<IConnection> CreateConnectionAsync()
        {
            if (_connection == null || !_connection.IsOpen)
            {
                await _semaphore.WaitAsync();
                try
                {
                    if (_connection == null || !_connection.IsOpen)
                    {
                        var factory = new ConnectionFactory
                        {
                            HostName = _settings.HostName,
                            Port = _settings.Port,
                            UserName = _settings.UserName,
                            Password = _settings.Password,
                            VirtualHost = _settings.VirtualHost,
                            AutomaticRecoveryEnabled = true,
                            NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
                        };

                        _connection = await factory.CreateConnectionAsync();
                    }
                }
                finally
                {
                    _semaphore.Release();
                }
            }

            return _connection;
        }

        public async Task<IChannel> CreateChannelAsync()
        {
            var connection = await CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();

            // Declare exchange
            await channel.ExchangeDeclareAsync(
                exchange: _settings.ExchangeName,
                type: ExchangeType.Topic,
                durable: _settings.Durable,
                autoDelete: _settings.AutoDelete);

            return channel;
        }

        public async ValueTask DisposeAsync()
        {
            if (_connection != null)
            {
                await _connection.CloseAsync();
                _connection.Dispose();
            }
            _semaphore.Dispose();
        }
    }
}
