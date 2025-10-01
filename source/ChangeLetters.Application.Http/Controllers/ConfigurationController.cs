using ChangeLetters.Domain.IO;

namespace ChangeLetters.Application.Http.Controllers;

/// <summary> 
/// Access point for the FTP configuration
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ConfigurationController(
    IConfigurationIo _configurationIo,
    ILogger<ConfigurationController> _log) : ControllerBase
{
    /// <summary>Gets the FTP configuration.</summary>
    [HttpGet]
    [ProducesResponseType<Configuration>(StatusCodes.Status200OK)]
    public ActionResult<Configuration> Get()
    {
        var config = _configurationIo.GetConfiguration();
        return Ok(config);
    }

    /// <summary>Saves the specified configuration.</summary>
    /// <param name="configuration">The configuration.</param>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Save([FromBody] Configuration configuration)
    {
        _configurationIo.SaveConfiguration(configuration);
        _log.LogInformation("Configuration saved successfully.");
        return NoContent();
    }
}
