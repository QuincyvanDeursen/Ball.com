using Microsoft.Extensions.Options;
using Shared.Infrastructure.Messaging;
using Shared.Infrastructure.Messaging.Configuration;
using Shared.Infrastructure.Messaging.Interfaces;

namespace CustomerService.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRabbitMqMessaging(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RabbitMqSettings>(configuration.GetSection(RabbitMqSettings.SectionName));

            services.AddSingleton<IRabbitMqConnectionFactory, RabbitMqConnectionFactory>();

            services.AddSingleton<IEventDispatcher, EventDispatcher>();

            // Dit moet je toevoegen als je die interface gebruikt!
            services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();

            // Event handlers
            // services.AddScoped<IEventHandler<CustomerUpdatedEvent>, PaymentCustomerUpdatedHandler>();

            services.AddSingleton(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<RabbitMqSettings>>().Value;
                var factory = sp.GetRequiredService<IRabbitMqConnectionFactory>();
                var dispatcher = sp.GetRequiredService<IEventDispatcher>();
                var logger = sp.GetRequiredService<ILogger<RabbitMqEventConsumer>>();

                return new RabbitMqEventConsumer(factory, dispatcher, logger, settings.ServiceName);
            });

            //only needed for listening
            //services.AddHostedService<RabbitMqBackgroundService>();

            return services;
        }
    }

}
