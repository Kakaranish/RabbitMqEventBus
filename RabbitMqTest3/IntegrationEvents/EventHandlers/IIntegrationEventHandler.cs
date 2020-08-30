using System.Threading.Tasks;
using RabbitMqTest3.IntegrationEvents.EventTypes;

namespace RabbitMqTest3.IntegrationEvents.EventHandlers
{
    public interface IIntegrationEventHandler<in TEvent> : IIntegrationEventHandler 
        where TEvent : IntegrationEventBase
    {
        Task Handle(TEvent @event);
    }

    public interface IIntegrationEventHandler
    {
    }
}
