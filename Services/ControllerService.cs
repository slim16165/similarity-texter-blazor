using ChatGPT_Splitter_Blazor_New.Pages.TextComparer.Model.Comparison;
using ChatGPT_Splitter_Blazor_New.Pages.TextComparer.Model.TextProcessing;
using ChatGPT_Splitter_Blazor_New.TextComparer.Services.Interfaces;

namespace ChatGPT_Splitter_Blazor_New.Pages.TextComparer.Services
{
    public class ControllerService : IControllerService
    {
        private readonly IStorageService _storageService;
        private readonly SimTexter _simTexter;

        public ControllerService(IStorageService storageService, SimTexter simTexter)
        {
            _storageService = storageService;
            _simTexter = simTexter;
            Instance = this;
        }

        public static ControllerService Instance { get; set; }

        public async Task<List<List<MatchSegment>>> CompareTextsAsync(string input1, string input2)
        {
            if (string.IsNullOrWhiteSpace(input1) || string.IsNullOrWhiteSpace(input2))
            {
                throw new ArgumentException("Uno o entrambi i testi sono vuoti.");
            }

            _simTexter.IgnoreLetterCase = _storageService.GetItemValueByKey<bool>("ignoreLetterCase");
            _simTexter.IgnoreNumbers = _storageService.GetItemValueByKey<bool>("ignoreNumbers");
            _simTexter.IgnorePunctuation = _storageService.GetItemValueByKey<bool>("ignorePunctuation");
            _simTexter.ReplaceUmlaut = _storageService.GetItemValueByKey<bool>("replaceUmlaut");
            _simTexter.MinMatchLength = _storageService.GetItemValueByKey<int>("minMatchLength");

            var inputTexts = new List<MyInputText>
            {
                new MyInputText { Mode = "Text", Text = input1 },
                new MyInputText { Mode = "Text", Text = input2 }
            };

            try
            {
                return await _simTexter.CompareAsync(inputTexts);
            }
            catch (Exception ex)
            {
                throw new Exception($"Errore durante il confronto: {ex.Message}", ex);
            }
        }
    }
}