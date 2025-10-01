using ChangeLetters.Database.Repositories;
using ChangeLetters.Domain.Handlers;
using ChangeLetters.Shared;
using ChangeLetters.Models.Converters;
using Microsoft.AspNetCore.Mvc;

namespace ChangeLetters.Controllers;

/// <summary> 
/// Controls vocabulary access.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class VocabularyController(
    IVocabularyHandler _handler,
    IVocabularyRepository _repository,
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
        await _repository.RecreateAllItemsAsync(allEntities).ConfigureAwait(false);

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
        await _repository.UpsertEntriesAsync(entities).ConfigureAwait(false);
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
            var items = await _repository.GetAllItemsAsync(HttpContext?.RequestAborted ?? CancellationToken.None)
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
        var resultSet = await _handler.GetRequiredVocabularyAsync(unknownWords, HttpContext?.RequestAborted ?? CancellationToken.None)
            .ConfigureAwait(false);
        return Ok(resultSet);
    }

    /// <summary>Gets the required words by a bodied mass data access.</summary>
    /// <param name="unknownWords">The unknown words.</param>
    [HttpGet("Unknowns/MassData")]
    [ProducesResponseType<VocabularyEntry[]>(StatusCodes.Status200OK)]
    public async Task<ActionResult<VocabularyEntry[]>> GetRequiredWordsMassData([FromBody] string[] unknownWords)
    {
        _log.LogInformation("GetRequiredWordsMassData called with {count} unknown words", unknownWords.Length);
        var resultSet = await _handler.GetRequiredVocabularyAsync(unknownWords, HttpContext?.RequestAborted ?? CancellationToken.None)
            .ConfigureAwait(false);
        return Ok(resultSet);
    }

}
