using Microsoft.AspNetCore.Mvc;
using ServerlessLogin.Dtos.General;

namespace ServerlessLogin.Controllers
{

    [Route("/general")]
    [ApiController]
    public class GeneralController : Controller
    {
        [HttpGet("healthcheck")]
        [ProducesResponseType(200)]
        public ActionResult HealthCheck()
        {
            var response = new HealthCheckResponseDto();

            return Ok(response);
        }
    }
}
