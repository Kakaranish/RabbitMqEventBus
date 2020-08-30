using Microsoft.AspNetCore.Mvc;

namespace RabbitMqTest3.Controllers
{
    [ApiController]
    public class ProductsController : Controller
    {
        [HttpPost("create")]
        public IActionResult CreateProduct()
        {
            return Ok();
        }
    }
}
