using System;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using RabbitMqTest3.IntegrationEvents.EventHandlers;

namespace RabbitMqTest3.EventBus
{
    public class IntegrationEventProcessor : IIntegrationEventProcessor
    {
        private readonly IEventBusSubscriptionManager _eventBusSubscriptionManager;
        private readonly ILifetimeScope _lifetimeScope;
        private readonly ILogger<IIntegrationEventProcessor> _logger;

        public IntegrationEventProcessor(IEventBusSubscriptionManager eventBusSubscriptionManager, ILifetimeScope lifetimeScope, ILogger<IIntegrationEventProcessor> logger)
        {
            _eventBusSubscriptionManager = eventBusSubscriptionManager ?? throw new ArgumentNullException(nameof(eventBusSubscriptionManager));
            _lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Process(BasicDeliverEventArgs eventMessage)
        {
            var eventName = eventMessage.RoutingKey;
            if (!_eventBusSubscriptionManager.HasSubscribedEvent(eventName))
            {
                _logger.LogWarning($"Event '{eventName}' cannot be processed because it is not subscribed");
                return false;
            }

            var eventType = _eventBusSubscriptionManager.GetEventType(eventName);
            var eventBody = Encoding.UTF8.GetString(eventMessage.Body.ToArray());
            var @event = JsonConvert.DeserializeObject(eventBody, eventType);

            using (var scope = _lifetimeScope.BeginLifetimeScope())
            {
                var handlerType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                var eventHandler = scope.Resolve(handlerType);

                await(Task) handlerType.GetMethod("Handle")?.Invoke(eventHandler, new[] { @event });
            }

            return true;
        }
    }
}