using System;

namespace RabbitMqTest3.IntegrationEvents.EventTypes
{
    public class ProductCreatedIntegrationEvent : IntegrationEventBase
    {
        public Guid ProductId { get; }
        public string Name { get; }
        public decimal Price { get; }
        public int Stock { get; }

        public ProductCreatedIntegrationEvent(Guid id, DateTime creationDate, string name, Guid productId, decimal price, int stock) : base(id, creationDate)
        {
            Name = name;
            ProductId = productId;
            Price = price;
            Stock = stock;
        }
    }
}
