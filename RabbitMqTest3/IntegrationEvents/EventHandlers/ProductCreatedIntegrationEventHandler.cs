using Microsoft.Extensions.Logging;
using RabbitMqTest3.IntegrationEvents.EventTypes;
using System;
using System.Threading.Tasks;

namespace RabbitMqTest3.IntegrationEvents.EventHandlers
{
    public class ProductCreatedIntegrationEventHandler : IIntegrationEventHandler<ProductCreatedIntegrationEvent>
    {
        private readonly ILogger<ProductCreatedIntegrationEventHandler> _logger;

        public ProductCreatedIntegrationEventHandler(ILogger<ProductCreatedIntegrationEventHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(ProductCreatedIntegrationEvent @event)
        {
            _logger.LogInformation($"Handling '{@event.GetType().Name}' event for product {@event.Name}");
        }
    }
}
