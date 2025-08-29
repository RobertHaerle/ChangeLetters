using ChangeLetters.DTOs;
using ChangeLetters.Models;
using ChangeLetters.Handlers;
using ChangeLetters.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ChangeLetters.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VocabularyController(
    IVocabularyHandler _handler,
    IVocabularyRepository _repository,
    ILogger<VocabularyController> _log) : Controller
{
    [HttpPost("RebuildAll")]
    public async Task<IActionResult> RebuildAllItemsAsync([FromBody] IList<VocabularyEntry> entries)
    {
        _log.LogInformation("RebuildAllItemsAsync called with {count} entries", entries.Count);
        var allEntities = entries
            .Select(x => x.ToModel()).ToArray();
        await _repository.RecreateAllItemsAsync(allEntities).ConfigureAwait(false);

        return Ok();
    }

    [HttpPut("Upsert")]
    public async Task<IActionResult> UpsertEntriesAsync([FromBody] IList<VocabularyEntry> entries)
    {
        _log.LogInformation("UpsertEntriesAsync called with {count} entries", entries.Count);
        var entities = entries
            .Select(x => x.ToModel()).ToArray();
        await _repository.UpsertEntriesAsync(entities).ConfigureAwait(false);
        return Ok();
    }

    [HttpGet]
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

    [HttpGet("Unknowns")]
    public async Task<ActionResult<VocabularyEntry[]>> GetRequiredWords([FromQuery] string[] unknownWords)
    {
        _log.LogInformation("GetRequiredWords called with {count} unknown words", unknownWords.Length);
        var resultSet = await _handler.GetRequiredVocabularyAsync(unknownWords, HttpContext?.RequestAborted ?? CancellationToken.None)
            .ConfigureAwait(false);
        return Ok(resultSet);
    }

    [HttpGet("Unknowns/MassData")]
    public async Task<ActionResult<VocabularyEntry[]>> GetRequiredWordsMassData([FromBody] string[] unknownWords)
    {
        _log.LogInformation("GetRequiredWordsMassData called with {count} unknown words", unknownWords.Length);
        var resultSet = await _handler.GetRequiredVocabularyAsync(unknownWords, HttpContext?.RequestAborted ?? CancellationToken.None)
            .ConfigureAwait(false);
        return Ok(resultSet);
    }

}
