using RabbitMqTest3.IntegrationEvents.EventTypes;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace RabbitMqTest3.IntegrationEvents.EventHandlers
{
    public class ProductRemovedIntegrationEventHandler : IIntegrationEventHandler<ProductRemovedIntegrationEvent>
    {
        private readonly ILogger<ProductRemovedIntegrationEventHandler> _logger;

        public ProductRemovedIntegrationEventHandler(ILogger<ProductRemovedIntegrationEventHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(ProductRemovedIntegrationEvent @event)
        {
            _logger.LogInformation($"Handling {@event.GetType().Name} event for product {@event.Name}");
        }
    }
}
