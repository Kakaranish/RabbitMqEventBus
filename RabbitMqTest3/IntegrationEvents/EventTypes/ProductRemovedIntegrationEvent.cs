using System;

namespace RabbitMqTest3.IntegrationEvents.EventTypes
{
    public class ProductRemovedIntegrationEvent : IntegrationEventBase
    {
        public Guid ProductId { get; }
        public string Name { get; }
        public ProductRemovedIntegrationEvent(Guid id, DateTime creationDate, string name, Guid productId) : base(id, creationDate)
        {
            Name = name;
            ProductId = productId;
        }
    }
}
