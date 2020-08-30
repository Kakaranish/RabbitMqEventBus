using RabbitMqTest3.IntegrationEvents.EventTypes;
using System;
using System.Threading.Tasks;

namespace RabbitMqTest3.IntegrationEvents.EventHandlers
{
    public class ProductRemovedIntegrationEventHandler : IIntegrationEventHandler<ProductRemovedIntegrationEvent>
    {
        public Task Handle(ProductRemovedIntegrationEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}
