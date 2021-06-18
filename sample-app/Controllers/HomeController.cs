using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace sample_app.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [Route("/get-random-number")]
        public async Task<IActionResult> Index([FromServices] IHttpClientFactory clientFactory)
        {
            _logger.LogInformation("Request for random number!");

            var number = new Random().Next(0, 100);

            var messageClient = clientFactory.CreateClient("message");
            await messageClient.PostAsJsonAsync("/sb/messaging-sidecar-topic", number);

            return Ok(number);
        }

        [Route("handle-message")]
        public IActionResult HandleMessage()
        {
            _logger.LogInformation("Message received.");
            return Ok();
        }
    }
}
