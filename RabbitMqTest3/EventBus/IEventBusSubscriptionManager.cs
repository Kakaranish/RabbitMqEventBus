using System;
using RabbitMqTest3.IntegrationEvents.EventHandlers;
using RabbitMqTest3.IntegrationEvents.EventTypes;

namespace RabbitMqTest3.EventBus
{
    public interface IEventBusSubscriptionManager
    {
        bool AddSubscription<TEvent, TEventHandler>()
            where TEvent : IntegrationEventBase
            where TEventHandler : IIntegrationEventHandler<TEvent>;

        bool HasSubscribedEvent(string eventName);

        Type GetHandlerTypeForEvent(string eventName);

        Type GetEventType(string eventName);
    }
}
