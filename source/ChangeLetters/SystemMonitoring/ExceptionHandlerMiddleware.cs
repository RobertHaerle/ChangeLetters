using System.Net;
using System.Text.Json;

namespace ChangeLetters.SystemMonitoring;
/// <summary> 
/// Class ExceptionHandlerMiddleware. 
/// </summary>
public class ExceptionHandlerMiddleware
{
    private readonly bool _isProduction;
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _log;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionHandlerMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next request.</param>
    /// <param name="log">The log.</param>
    /// <param name="webHostEnvironment">The web host environment.</param>
    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> log, IWebHostEnvironment webHostEnvironment)
    {
        _log = log;
        _next = next;
        var environment = webHostEnvironment.EnvironmentName;

        _isProduction = environment.Equals("Production", StringComparison.InvariantCultureIgnoreCase);
    }

    /// <summary>Invokes the specified HTTP context.</summary>
    /// <param name="httpContext">The HTTP context.</param>
    /// <returns>See description.</returns>
    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
            if (httpContext.Response.StatusCode == StatusCodes.Status404NotFound)
            {
                var caller = httpContext.Connection.RemoteIpAddress;
                var http = httpContext.Request.Path;
                _log.LogInformation($"could not handle request \"{http}\" from {caller}");
            }
        }
        catch (Exception ex)
        {
            _log.LogError(ex, $"requested {httpContext.Request.Path} by {httpContext.Connection.RemoteIpAddress}");
            var result = _isProduction
                ? JsonSerializer.Serialize(new { ex.Message, Type = ex.GetType().FullName })
                : JsonSerializer.Serialize(new { Message = ex.ToString(), Type = ex.GetType().FullName, ex.StackTrace });

            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            await httpContext.Response.WriteAsync(result);
        }
    }
}