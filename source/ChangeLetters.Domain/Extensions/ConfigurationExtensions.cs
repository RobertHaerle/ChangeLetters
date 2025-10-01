using ChangeLetters.Domain.Configurations;
using Microsoft.Extensions.Configuration;

namespace ChangeLetters.Domain.Extensions;

/// <summary>
///     Configuration extension methods
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>Deserializes the specified key.</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="config">The configuration.</param>
    /// <param name="key">The key.</param>
    /// <returns>T.</returns>
    public static T Deserialize<T>(this IConfiguration config, string? key = null) where T : class, new()
    {
        if (key.IsNullOrEmpty())
        {
            var att = typeof(T).GetCustomAttributes(typeof(AppConfigAttribute), true).FirstOrDefault();
            if (att != null)
            {
                key = ((AppConfigAttribute)att).Name;
                if (key.IsNullOrEmpty())
                    key = typeof(T).Name;
            }
        }

        T instance = new();
        config.Bind(key!, instance);
        return instance;
    }
}
