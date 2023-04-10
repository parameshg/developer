using Microsoft.AspNetCore.Mvc;

namespace Developer.Api.Controllers
{
    [Route("")]
    public class HomeController : ControllerBase
    {
        [HttpGet("")]
        public object Get()
        {
            return new
            {
                timestamp = DateTime.Now
            };
        }
    }
}