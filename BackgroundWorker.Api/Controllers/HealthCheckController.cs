using Microsoft.AspNetCore.Mvc;

namespace BackgroundWorker.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class HealthCheckController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok($"Ok. {DateTime.Now}");
}