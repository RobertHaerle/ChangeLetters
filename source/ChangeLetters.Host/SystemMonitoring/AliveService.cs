namespace ChangeLetters.SystemMonitoring;

/// <summary> 
/// Class AliveService.
/// Inherits from <see cref="BackgroundService" />
/// </summary>
public class AliveService : BackgroundService
{
    private readonly bool _isActive;
    private readonly TimeSpan _delay;
    private readonly ILogger<AliveService> _log;

    /// <summary> 
    /// Class AliveService.
    /// Inherits from <see cref="BackgroundService" />
    /// </summary>
    public AliveService(ILogger<AliveService> log,
        IConfiguration configuration)
    {
        _log = log;
        _isActive = configuration.GetValue<bool>("Alive:IsActive", false);
        var delay = configuration.GetValue<int>("Alive:IntervalSeconds", 5);
        delay = delay < 5 ? 5 : delay;
        _delay = TimeSpan.FromSeconds(delay);
        _log.LogInformation("Alive service is active: {IsActive}, interval: {Interval} seconds", _isActive, delay);
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_isActive)
        {
            _log.LogInformation("Alive service is not active.");
            return;
        }
        while (!stoppingToken.IsCancellationRequested)
        {
            await DelayAsync(stoppingToken);
            _log.LogInformation("Alive");
        }
    }

    private async Task DelayAsync(CancellationToken stoppingToken)
    {
        try
        {
            await Task.Delay(_delay, stoppingToken);
        }
        catch (TaskCanceledException )
        {
           _log.LogInformation("stop requested.");
        }
    }
}