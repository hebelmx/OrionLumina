using System.Text.Json;

namespace Orion.Lumina.Domain;

public static class JsonHelper
{
    public static List<T> ConvertFromJson<T>(this string json)
    {
        if (string.IsNullOrEmpty(json))
            throw new ArgumentNullException(nameof(json), "JSON input cannot be null or empty");

        return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
    }
}