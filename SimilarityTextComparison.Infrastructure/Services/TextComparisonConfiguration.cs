namespace SimilarityTextComparison.Infrastructure.Services;

public class TextComparisonConfiguration
{
    public bool IgnoreLetterCase { get; private set; }
    public bool IgnoreNumbers { get; private set; }
    public bool IgnorePunctuation { get; private set; }
    public bool ReplaceUmlaut { get; private set; }

    private int _minMatchLength;

    public int MinMatchLength
    {
        get => _minMatchLength;
        private set => _minMatchLength = Math.Max(value, 2);
    }

    public bool IsHtmlInput { get; private set; }

    private readonly IStorageService _storage;

    public TextComparisonConfiguration(IStorageService storage)
    {
        _storage = storage ?? throw new ArgumentNullException(nameof(storage));
    }

    public async Task InitializeAsync()
    {
        IgnoreLetterCase = await _storage.GetItemAsync<bool>("ignoreLetterCase");
        IgnoreNumbers = await _storage.GetItemAsync<bool>("ignoreNumbers");
        IgnorePunctuation = await _storage.GetItemAsync<bool>("ignorePunctuation");
        ReplaceUmlaut = await _storage.GetItemAsync<bool>("replaceUmlaut");
        MinMatchLength = await _storage.GetItemAsync<int>("minMatchLength");
        IsHtmlInput = await _storage.GetItemAsync<bool>("isHtmlInput");
    }
}