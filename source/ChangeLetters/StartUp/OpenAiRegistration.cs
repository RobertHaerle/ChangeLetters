using OpenAI.Chat;
using ChangeLetters.Extensions;
using ChangeLetters.Configurations;

namespace ChangeLetters.StartUp;

/// <summary> 
/// Class OpenAiRegistration. 
/// </summary>
public static class OpenAiRegistration
{
    /// <summary>Adds the <see cref="ChatClient" /> to the <see cref="IServiceCollection" />.</summary>
    /// <param name="services">The services.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns><see cref="IServiceCollection" />.</returns>
    public static IServiceCollection AddOpenAI(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.Deserialize<OpenAiSettings>();
        var chatClient = new ChatClient(settings.Model, settings.ApiKey);
        services.AddSingleton(chatClient);
        services.AddSingleton(settings);
        return services;
    }

}
