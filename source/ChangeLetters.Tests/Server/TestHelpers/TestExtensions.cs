using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChangeLetters.Tests.Server.TestHelpers;

public static class TestExtensions
{
    public static List<T> LoadFromJson<T>(string json)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            ReferenceHandler = ReferenceHandler.Preserve
        };
        return JsonSerializer.Deserialize<List<T>>(json, options)!;
    }
}
