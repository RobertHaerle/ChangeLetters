using ChangeLetters.Domain.Handlers;
using ChangeLetters.Models.Converters;

namespace ChangeLetters.Application.Http.Controllers;

/// <summary> 
/// Controls vocabulary access.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class VocabularyController(
    IVocabularyHandler _handler,
    ILogger<VocabularyController> _log) : Controller
{
    /// <summary>Rebuild all items as an asynchronous operation.</summary>
    /// <param name="entries">The entries.</param>
    [HttpPost("RebuildAll")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RebuildAllItemsAsync([FromBody] IList<VocabularyEntry> entries)
    {
        _log.LogInformation("RebuildAllItemsAsync called with {count} entries", entries.Count);
        var allEntities = entries
            .Select(x => x.ToModel()).ToArray();
        try
        {
            await _handler.RecreateAllItemsAsync(allEntities, HttpContext.RequestAborted).ConfigureAwait(false);
        }
        catch (TaskCanceledException)
        {
            _log.LogError("RebuildAllItemsAsync cancelled by caller.");
        }

        return Ok();
    }

    /// <summary>Inserts or update entries as an asynchronous operation.</summary>
    /// <param name="entries">The entries.</param>
    [HttpPut("Upsert")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpsertEntriesAsync([FromBody] IList<VocabularyEntry> entries)
    {
        _log.LogInformation("UpsertEntriesAsync called with {count} entries", entries.Count);
        var entities = entries
            .Select(x => x.ToModel()).ToArray();
        try
        {
            await _handler.UpsertEntriesAsync(entities, HttpContext.RequestAborted).ConfigureAwait(false);
        }
        catch (TaskCanceledException)
        {
            _log.LogError("UpsertEntriesAsync cancelled by caller.");
        }
        return Ok();
    }

    /// <summary>Get all items as an asynchronous operation.</summary>
    [HttpGet]
    [ProducesResponseType<VocabularyEntry[]>(StatusCodes.Status200OK)]
    public async Task<ActionResult<VocabularyEntry[]>> GetAllItemsAsync()
    {
        _log.LogInformation("GetAllItemsAsync called");
        try
        {
            var items = await _handler.GetAllItemsAsync(HttpContext.RequestAborted)
                .ConfigureAwait(false);
            var dtos = items.Select(x => x.ToDto())
                .ToArray();
            return Ok(dtos);
        }
        catch (TaskCanceledException)
        {
            return Ok(Array.Empty<VocabularyEntry>());
        }
    }

    /// <summary>Gets the required words.</summary>
    /// <param name="unknownWords">The unknown words.</param>
    [HttpGet("Unknowns")]
    [ProducesResponseType<VocabularyEntry[]>(StatusCodes.Status200OK)]
    public async Task<ActionResult<VocabularyEntry[]>> GetRequiredWords([FromQuery] string[] unknownWords)
    {
        _log.LogInformation("GetRequiredWords called with {count} unknown words", unknownWords.Length);
        try
        {
            var resultSet = await _handler.GetRequiredVocabularyAsync(unknownWords, HttpContext.RequestAborted)
                .ConfigureAwait(false);
            return Ok(resultSet);
        }
        catch (TaskCanceledException)
        {
            _log.LogError("UpsertEntriesAsync cancelled by caller.");
            return Ok(Array.Empty<VocabularyEntry>());
        }
    }

    /// <summary>Gets the required words by a bodied mass data access.</summary>
    /// <param name="unknownWords">The unknown words.</param>
    [HttpGet("Unknowns/MassData")]
    [ProducesResponseType<VocabularyEntry[]>(StatusCodes.Status200OK)]
    public async Task<ActionResult<VocabularyEntry[]>> GetRequiredWordsMassData([FromBody] string[] unknownWords)
    {
        _log.LogInformation("GetRequiredWordsMassData called with {count} unknown words", unknownWords.Length);
        try
        {
            var resultSet = await _handler.GetRequiredVocabularyAsync(unknownWords, HttpContext.RequestAborted)
                .ConfigureAwait(false);
            return Ok(resultSet);
        }
        catch (TaskCanceledException)
        {
            _log.LogError("GetRequiredWordsMassData cancelled by caller.");
            return Ok(Array.Empty<VocabularyEntry>());
        }
    }
}
