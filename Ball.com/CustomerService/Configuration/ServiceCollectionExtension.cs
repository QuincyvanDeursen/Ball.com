using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

// shared‐infra types
using Shared.Infrastructure.Messaging;
using Shared.Infrastructure.Messaging.Configuration;
using Shared.Infrastructure.Messaging.Events;
using Shared.Infrastructure.Messaging.Interfaces;

// your handlers
using CustomerService.EventHandlers;
using Shared.Infrastructure.Messaging.BackGroundService;
using Shared.Infrastructure.Messaging.Events.Interfaces;

namespace CustomerService.Configuration
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddRabbitMqMessaging(this IServiceCollection services, IConfiguration configuration)
        {
            // ── 1) Bind RabbitMQ settings from appsettings.json ───────────────────
            services.Configure<RabbitMqSettings>(
                configuration.GetSection(RabbitMqSettings.SectionName));

            // ── 2) Core RabbitMQ plumbing ────────────────────────────────────────
            services.AddSingleton<IRabbitMqConnectionFactory, RabbitMqConnectionFactory>();
            services.AddSingleton<IEventDispatcher, EventDispatcher>();
            services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();

            // ── 3) Register your two handlers ────────────────────────────────────
            services.AddScoped<IEventHandler<CustomerCreatedEvent>, CustomerCreatedEventHandler>();
            services.AddScoped<IEventHandler<CustomerUpdatedEvent>, CustomerUpdatedEventHandler>();

            // ── 4) Build the consumer & dispatcher ──────────────────────────────
            services.AddSingleton(sp =>
            {
                var settings   = sp.GetRequiredService<IOptions<RabbitMqSettings>>().Value;
                var factory    = sp.GetRequiredService<IRabbitMqConnectionFactory>();
                var dispatcher = sp.GetRequiredService<IEventDispatcher>();
                var logger     = sp.GetRequiredService<ILogger<RabbitMqEventConsumer>>();

                return new RabbitMqEventConsumer(factory, dispatcher, logger, settings.ServiceName);
            });

            // ── 5) Run the consumer in the background ────────────────────────────
            //services.AddHostedService<RabbitMqBackgroundService>();

            return services;
        }
    }
}
