using SimilarityTextComparison.Core.Interfaces;

namespace SimilarityTextComparison.Core.Services.Configuration
{
    public class Configuration
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
    }
}