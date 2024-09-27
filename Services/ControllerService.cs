namespace ChatGPT_Splitter_Blazor_New.Pages.TextComparer.Services;

using System.Threading.Tasks;
using Model.TextProcessing;
using ChatGPT_Splitter_Blazor_New.TextComparer.Services.Interfaces;

public class ControllerService : IControllerService
{
    private readonly IStorageService _storageService;
    private readonly SimTexter _simTexter;

    // Inietta le dipendenze necessarie, incluso SimTexter e StorageService
    public ControllerService(IStorageService storageService, SimTexter simTexter)
    {
        _storageService = storageService;
        _simTexter = simTexter;
    }

    public static ControllerService Instance { get; set; }

    // Metodo per confrontare due testi utilizzando SimTexter
    public async Task<string> CompareTextsAsync(string input1, string input2)
    {
        if (string.IsNullOrWhiteSpace(input1) || string.IsNullOrWhiteSpace(input2))
        {
            return "Uno o entrambi i testi sono vuoti.";
        }

        // Imposta SimTexter in base alle impostazioni recuperate dallo StorageService
        _simTexter.IgnoreLetterCase = _storageService.GetItemValueByKey<bool>("ignoreLetterCase");
        _simTexter.IgnoreNumbers = _storageService.GetItemValueByKey<bool>("ignoreNumbers");
        _simTexter.IgnorePunctuation = _storageService.GetItemValueByKey<bool>("ignorePunctuation");
        _simTexter.ReplaceUmlaut = _storageService.GetItemValueByKey<bool>("replaceUmlaut");
        _simTexter.MinMatchLength = _storageService.GetItemValueByKey<int>("minMatchLength");

        // Crea due istanze di MyInputText e imposta il loro contenuto
        var inputTexts = new List<MyInputText>
        {
            new MyInputText("Text", null, input1),
            new MyInputText("Text", null, input2)
        };

        try
        {
            // Esegue il confronto utilizzando SimTexter
            var result = await _simTexter.CompareAsync(inputTexts);

            if (result.Count > 0)
            {
                return "Sono state trovate somiglianze tra i testi.";
            }
            else
            {
                return "Non sono state trovate somiglianze.";
            }
        }
        catch (Exception ex)
        {
            return $"Errore durante il confronto: {ex.Message}";
        }
    }
}