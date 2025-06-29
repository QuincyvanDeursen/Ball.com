using Microsoft.Extensions.Options;
using OrderService.Handlers;
using OrderService.Services;
using Shared.Infrastructure.Messaging;
using Shared.Infrastructure.Messaging.BackGroundService;
using Shared.Infrastructure.Messaging.Configuration;
using Shared.Infrastructure.Messaging.Events;
using Shared.Infrastructure.Messaging.Events.Interfaces;
using Shared.Infrastructure.Messaging.Events.Items;
using Shared.Infrastructure.Messaging.Events.Orders;
using Shared.Infrastructure.Messaging.Events.Payments;
using Shared.Infrastructure.Messaging.Interfaces;

namespace OrderService.Configuration
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
            services.AddScoped<IEventHandler<CustomerCreatedEvent>, CustomerCreatedHandler>();
            services.AddScoped<IEventHandler<CustomerUpdatedEvent>, CustomerUpdatedHandler>();
            services.AddScoped<IEventHandler<ItemCreatedEvent>, ItemCreatedHandler>();
            services.AddScoped<IEventHandler<ItemUpdatedEvent>, ItemUpdatedHandler>();
            services.AddScoped<IEventHandler<PaymentCancelledEvent>, PaymentCancelledHandler>();
            services.AddScoped<IEventHandler<PaymentPaidEvent>, PaymentPaidHandler>();
            services.AddScoped<IEventHandler<OrderValidatedEvent>, OrderValidatedHandler>();
            services.AddScoped<IEventHandler<OrderCancelledEvent>, OrderCancelledHandler>();

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
