using Microsoft.Extensions.Logging;
using RabbitMqTest3.IntegrationEvents.EventHandlers;
using RabbitMqTest3.IntegrationEvents.EventTypes;
using System;
using System.Collections.Generic;

namespace RabbitMqTest3.EventBus
{
    public class EventBusSubscriptionManager : IEventBusSubscriptionManager
    {
        private readonly ILogger<IEventBusSubscriptionManager> _logger;
        private readonly IDictionary<EventSubscription, Type> _eventHandlers;

        public EventBusSubscriptionManager(ILogger<IEventBusSubscriptionManager> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _eventHandlers = new Dictionary<EventSubscription, Type>();
        }

        public bool AddSubscription<TEvent, TEventHandler>()
            where TEvent : IntegrationEventBase
            where TEventHandler : IIntegrationEventHandler<TEvent>
        {
            var eventName = typeof(TEvent).Name;
            var eventSubscription = new EventSubscription(eventName, typeof(TEvent));

            if (_eventHandlers.ContainsKey(eventSubscription))
            {
                _logger.LogWarning($"Event '{eventName}' is already subscribed");
                return false;
            }

            _eventHandlers.Add(eventSubscription, typeof(TEventHandler));
            _logger.LogInformation($"Event '{eventName}' successfully subscribed.");

            return true;
        }
    }
}
