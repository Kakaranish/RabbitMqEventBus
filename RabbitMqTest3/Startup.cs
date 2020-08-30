using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMqTest3.EventBus;
using RabbitMqTest3.IntegrationEvents.EventHandlers;
using RabbitMqTest3.IntegrationEvents.EventTypes;

namespace RabbitMqTest3
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });

            services.Configure<RabbitMqConfig>(Configuration.GetSection("RabbitMq"));

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
            services.AddSingleton<IEventBus, EventBusRabbitMq>();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(IIntegrationEventHandler<>).Assembly)
                .AsClosedTypesOf(typeof(IIntegrationEventHandler<>));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });

            UseRabbitMq(app);
        }

        private void UseRabbitMq(IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            eventBus.Subscribe<ProductCreatedIntegrationEvent, IIntegrationEventHandler<ProductCreatedIntegrationEvent>>();
            eventBus.Subscribe<ProductRemovedIntegrationEvent, IIntegrationEventHandler<ProductRemovedIntegrationEvent>>();
            eventBus.StartConsuming();
        }
    }
}
