using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ChangeLetters.SystemMonitoring;

/// <summary> 
/// Class HealthCheck.
/// Implements <see cref="IHealthCheck" />
/// </summary>
public class HealthCheck (ILogger<HealthCheck> _log): IHealthCheck
{
    /// <inheritdoc />
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        _log.LogInformation("Health check executed.");
        return Task.FromResult(HealthCheckResult.Healthy());
    }
}