using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMqTest3.IntegrationEvents.EventHandlers;
using RabbitMqTest3.IntegrationEvents.EventTypes;

namespace RabbitMqTest3.EventBus
{
    public static class EventBusRabbitMqExtensions
    {
        public static IServiceCollection AddRabbitMqEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RabbitMqConfig>(configuration.GetSection("RabbitMq"));

            services.AddSingleton<IConnectionFactory>(serviceProvider =>
            {
                var rabbitMqConfig = serviceProvider.GetRequiredService<IOptionsMonitor<RabbitMqConfig>>().CurrentValue;
                return new ConnectionFactory
                {
                    DispatchConsumersAsync = true,
                    UserName = rabbitMqConfig.Username,
                    Password = rabbitMqConfig.Password,
                    Port = rabbitMqConfig.Port,
                    HostName = rabbitMqConfig.HostName,
                    VirtualHost = rabbitMqConfig.VirtualHost
                };
            });

            services.AddSingleton<IEventBusSubscriptionManager, EventBusSubscriptionManager>();
            services.AddSingleton<IPersistentRabbitMqConnection, PersistentRabbitMqConnection>();
            services.AddSingleton<IIntegrationEventProcessor, IntegrationEventProcessor>();
            services.AddSingleton<IEventBus, EventBusRabbitMq>();

            return services;
        }

        public static void UseRabbitMq(this IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            eventBus.Subscribe<ProductCreatedIntegrationEvent, IIntegrationEventHandler<ProductCreatedIntegrationEvent>>();
            eventBus.Subscribe<ProductRemovedIntegrationEvent, IIntegrationEventHandler<ProductRemovedIntegrationEvent>>();

            eventBus.StartConsuming();
        }
    }
}
