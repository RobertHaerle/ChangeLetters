namespace ChangeLetters.Configurations;

/// <summary>
/// Represents the OpenAI configuration settings loaded from appsettings or user secrets.
/// </summary>
[AppConfig("OpenAI")]
public class OpenAiSettings
{
    /// <summary>
    /// The API key used to authenticate requests to the OpenAI service.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// The model name to use for OpenAI requests (e.g., "gpt-3.5-turbo").
    /// </summary>
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// The maximum number of tokens allowed in a response.
    /// </summary>
    public int MaxTokens { get; set; }

    /// <summary>
    /// Controls the diversity via nucleus sampling.
    /// </summary>
    public double TopP { get; set; }

    /// <summary>
    /// Penalizes new tokens based on their frequency in the text so far.
    /// </summary>
    public double FrequencyPenalty { get; set; }

    /// <summary>
    /// Penalizes new tokens based on whether they appear in the text so far.
    /// </summary>
    public double PresencePenalty { get; set; }
}