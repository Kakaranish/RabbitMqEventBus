using RabbitMqTest3.IntegrationEvents.EventHandlers;
using RabbitMqTest3.IntegrationEvents.EventTypes;

namespace RabbitMqTest3.EventBus
{
    public interface IEventBus
    {
        void Publish(IntegrationEventBase @event);

        void Subscribe<TEvent, TEventHandler>()
            where TEvent : IntegrationEventBase
            where TEventHandler : IIntegrationEventHandler<TEvent>;

        void StartConsuming();

        bool IsConsuming { get; }
    }
}