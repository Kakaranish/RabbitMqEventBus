using RabbitMqTest3.IntegrationEvents.EventTypes;
using System;
using System.Threading.Tasks;

namespace RabbitMqTest3.IntegrationEvents.EventHandlers
{
    public class ProductCreatedIntegrationEventHandler : IIntegrationEventHandler<ProductCreatedIntegrationEvent>
    {
        public Task Handle(ProductCreatedIntegrationEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}
