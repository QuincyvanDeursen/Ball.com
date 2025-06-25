using Microsoft.Extensions.Options;
using PaymentService.Handlers;
using PaymentService.Services;
using Shared.Infrastructure.Messaging;
using Shared.Infrastructure.Messaging.Configuration;
using Shared.Infrastructure.Messaging.Events;
using Shared.Infrastructure.Messaging.Events.Interfaces;
using Shared.Infrastructure.Messaging.Events.Orders;
using Shared.Infrastructure.Messaging.Interfaces;

namespace PaymentService.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRabbitMqMessaging(this IServiceCollection services, IConfiguration configuration)
        {
            // Configuratie binden
            services.Configure<RabbitMqSettings>(configuration.GetSection(RabbitMqSettings.SectionName));

            // 1. Connection Factory (singleton)
            services.AddSingleton<IRabbitMqConnectionFactory, RabbitMqConnectionFactory>();

            // 2. Event Dispatcher (singleton)
            services.AddSingleton<IEventDispatcher, EventDispatcher>();
            services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();

            // 3. Event Handlers (scoped) dit zijn de events waar naar geluisterd wordt.
            services.AddScoped<IEventHandler<CustomerUpdatedEvent>, PaymentCustomerUpdateHandler>();
            services.AddScoped<IEventHandler<CustomerCreatedEvent>, PaymentCustomerCreatedHandler>();
            services.AddScoped<IEventHandler<OrderPlacedEvent>, PaymentOrderPlacedHandler>();
            services.AddScoped<IEventHandler<OrderCancelledEvent>, PaymentOrderCancelledHandler>();

            // 4. RabbitMQ Consumer (singleton)
            services.AddSingleton<IEventConsumer>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<RabbitMqSettings>>().Value;
                var factory = sp.GetRequiredService<IRabbitMqConnectionFactory>();
                var dispatcher = sp.GetRequiredService<IEventDispatcher>();
                var logger = sp.GetRequiredService<ILogger<RabbitMqEventConsumer>>();

                return new RabbitMqEventConsumer(factory, dispatcher, logger, settings.ServiceName);
            });

            // 5. BackgroundService die de consumer draait
            services.AddHostedService<RabbitMqBackgroundService>();

            return services;
        }
    }

}
