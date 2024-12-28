using System.Text.Json;

namespace SimilarityTextComparison.Infrastructure.Services;

public class FileStorageService : IStorageService
{
    private readonly string _filePath = "config.json";

    public async Task<T> GetItemAsync<T>(string key)
    {
        if (!File.Exists(_filePath))
            return default;

        var json = await File.ReadAllTextAsync(_filePath);
        var dictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
        return dictionary != null && dictionary.ContainsKey(key) ? (T)dictionary[key] : default;
    }

    public async Task SetItemAsync<T>(string key, T value)
    {
        Dictionary<string, object> dictionary;

        if (File.Exists(_filePath))
        {
            var json = await File.ReadAllTextAsync(_filePath);
            dictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(json) ?? new Dictionary<string, object>();
        }
        else
        {
            dictionary = new Dictionary<string, object>();
        }

        dictionary[key] = value;
        var updatedJson = JsonSerializer.Serialize(dictionary);
        await File.WriteAllTextAsync(_filePath, updatedJson);
    }
}