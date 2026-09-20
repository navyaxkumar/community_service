using DigitalShield.API.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DigitalShield.API.Controllers;

[ApiController]
[Route("api")]
public class ConfigHealthController : ControllerBase
{
    private readonly IOptions<ApplicationSettings> _applicationSettings;

    public ConfigHealthController(IOptions<ApplicationSettings> applicationSettings)
    {
        _applicationSettings = applicationSettings;
    }

    [HttpGet("health/config")]
    public IActionResult GetConfigHealth()
    {
        var settings = _applicationSettings.Value;

        return Ok(new
        {
            application = settings.Name,
            version = settings.Version,
            environment = settings.Environment,
            status = "ok"
        });
    }
}
