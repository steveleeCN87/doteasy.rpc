using Microsoft.AspNetCore.Mvc;

namespace doteasy.rpc.webserver.Controllers
{
    [Produces("application/json")]
    [Route("api/Health")]
    public class HealthController : Controller
    {
        [HttpGet]
        public IActionResult Get() => Ok("ok");
    }
}