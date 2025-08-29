using ChangeLetters.DTOs;
using ChangeLetters.Model;
using Microsoft.AspNetCore.Mvc;

namespace ChangeLetters.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConfigurationController(
    IConfigurationIo _configurationIo,
    ILogger<ConfigurationController> _log) : ControllerBase
{
    [HttpGet]
    public ActionResult<Configuration> Get()
    {
        var config = _configurationIo.GetConfiguration();
        return Ok(config);
    }

    [HttpPost]
    public IActionResult Save([FromBody] Configuration configuration)
    {
        _configurationIo.SaveConfiguration(configuration);
        _log.LogInformation("Configuration saved successfully.");
        return NoContent();
    }
}
