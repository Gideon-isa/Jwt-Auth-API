using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [Route("get")]
        [HttpGet]
        public ActionResult Get()
        {
            return Ok(Summaries);
       
        }

        [Route("getsecond")]
        [HttpGet]
        [Authorize(Roles ="ADMIN")]
        public ActionResult GetSecond()
        {
            return Ok(Summaries);

        }

        [Route("getthird")]
        [HttpGet]
        [Authorize(Roles = "OWNER")]
        public ActionResult GetThird()
        {
            return Ok(Summaries);

        }
    }
}
