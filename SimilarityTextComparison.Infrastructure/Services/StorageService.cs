using Blazored.LocalStorage;

namespace SimilarityTextComparison.Infrastructure.Services;

public class StorageService : IStorageService
{
    private readonly ILocalStorageService _localStorage;

    public StorageService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage ?? throw new ArgumentNullException(nameof(localStorage));
    }

    public async Task<T> GetItemAsync<T>(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("Key cannot be null or empty.", nameof(key));
        }

        return await _localStorage.GetItemAsync<T>(key);
    }

    public async Task SetItemAsync<T>(string key, T value)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("Key cannot be null or empty.", nameof(key));
        }

        await _localStorage.SetItemAsync(key, value);
    }
}