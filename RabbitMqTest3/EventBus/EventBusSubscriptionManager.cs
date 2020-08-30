using Microsoft.Extensions.Logging;
using RabbitMqTest3.IntegrationEvents.EventHandlers;
using RabbitMqTest3.IntegrationEvents.EventTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RabbitMqTest3.EventBus
{
    public class EventBusSubscriptionManager : IEventBusSubscriptionManager
    {
        private readonly ILogger<IEventBusSubscriptionManager> _logger;
        private readonly IList<Type> _subscribedEvents;
        private readonly IDictionary<string, Type> _handlerTypes;

        public EventBusSubscriptionManager(ILogger<IEventBusSubscriptionManager> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _handlerTypes = new Dictionary<string, Type>();
            _subscribedEvents = new List<Type>();
        }

        public bool AddSubscription<TEvent, TEventHandler>()
            where TEvent : IntegrationEventBase
            where TEventHandler : IIntegrationEventHandler<TEvent>
        {
            var eventName = typeof(TEvent).Name;
            if (_handlerTypes.ContainsKey(eventName))
            {
                _logger.LogWarning($"Event '{eventName}' is already subscribed");
                return false;
            }

            _handlerTypes.Add(eventName, typeof(TEventHandler));
            _subscribedEvents.Add(typeof(TEvent));

            _logger.LogInformation($"Event '{eventName}' successfully subscribed.");

            return true;
        }

        public bool HasSubscribedEvent(string eventName) => _handlerTypes.ContainsKey(eventName);

        public Type GetHandlerTypeForEvent(string eventName) => _handlerTypes[eventName];

        public Type GetEventType(string eventName) =>
            _subscribedEvents.FirstOrDefault(@event => @event.Name == eventName);
    }
}
