using SimilarityTextComparison.Blazor.Models;
using SimilarityTextComparison.Domain.Models.Comparison;
using SimilarityTextComparison.Infrastructure.Services;

namespace SimilarityTextComparison.Blazor.Services;

public class ControllerService : IControllerService
{
    private readonly IStorageService _storageService;
    private readonly ISimTexter _simTexter;

    public ControllerService(IStorageService storageService, ISimTexter simTexter)
    {
        _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        _simTexter = simTexter ?? throw new ArgumentNullException(nameof(simTexter));
    }

    public async Task<List<List<MatchSegment>>> CompareTextsAsync(string input1, string input2)
    {
        ValidateInputs(input1, input2);
        var inputTexts = CreateInputTexts(input1, input2);

        return await ExecuteComparisonAsync(inputTexts);
    }

    private static void ValidateInputs(string input1, string input2)
    {
        if (string.IsNullOrWhiteSpace(input1) || string.IsNullOrWhiteSpace(input2))
        {
            throw new ArgumentException("Uno o entrambi i testi sono vuoti.");
        }
    }

    private static List<MyInputText> CreateInputTexts(string input1, string input2)
    {
        return new List<MyInputText>
        {
            new MyInputText { Mode = "Text", Text = input1 },
            new MyInputText { Mode = "Text", Text = input2 }
        };
    }

    private async Task<List<List<MatchSegment>>> ExecuteComparisonAsync(List<MyInputText> inputTexts)
    {
        try
        {
            return await _simTexter.CompareAsync(inputTexts);
        }
        catch (Exception ex)
        {
            // Log dell'errore se necessario
            throw new Exception($"Errore durante il confronto: {ex.Message}", ex);
        }
    }

}

public interface ISimTexter
{
    Task<List<List<MatchSegment>>> CompareAsync(List<MyInputText> inputTexts);
}