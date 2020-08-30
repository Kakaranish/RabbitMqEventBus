using System;
using Microsoft.AspNetCore.Mvc;
using RabbitMqTest3.EventBus;
using RabbitMqTest3.IntegrationEvents.EventTypes;

namespace RabbitMqTest3.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : Controller
    {
        private readonly IEventBus _eventBus;

        public ProductsController(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        [HttpPost("create")]
        public IActionResult CreateProduct()
        {
            var productRemovedEvent = new ProductRemovedIntegrationEvent(Guid.NewGuid(), DateTime.UtcNow,
                "Some product", Guid.NewGuid());
            _eventBus.Publish(productRemovedEvent);
            
            return Ok();
        }
    }
}
