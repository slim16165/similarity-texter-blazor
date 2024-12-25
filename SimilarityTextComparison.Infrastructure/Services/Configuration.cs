using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace SimilarityTextComparison.Infrastructure.Services;

public class Configuration : IConfiguration
{
    public bool IgnoreLetterCase { get; private set; }
    public bool IgnoreNumbers { get; private set; }
    public bool IgnorePunctuation { get; private set; }
    public bool ReplaceUmlaut { get; private set; }
    public int MinMatchLength { get; private set; }

    private readonly IStorageService _storage;

    public Configuration(IStorageService storage)
    {
        _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        Task.Run(() => InitializeAsync());
    }

    public async Task InitializeAsync()
    {
        IgnoreLetterCase = await _storage.GetItemAsync<bool>("ignoreLetterCase");
        IgnoreNumbers = await _storage.GetItemAsync<bool>("ignoreNumbers");
        IgnorePunctuation = await _storage.GetItemAsync<bool>("ignorePunctuation");
        ReplaceUmlaut = await _storage.GetItemAsync<bool>("replaceUmlaut");
        MinMatchLength = await _storage.GetItemAsync<int>("minMatchLength");
    }

    public IConfigurationSection GetSection(string key)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IConfigurationSection> GetChildren()
    {
        throw new NotImplementedException();
    }

    public IChangeToken GetReloadToken()
    {
        throw new NotImplementedException();
    }

    public string? this[string key]
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }
}