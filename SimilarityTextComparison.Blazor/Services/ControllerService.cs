using SimilarityTextComparison.Application.Interfaces;
using SimilarityTextComparison.Blazor.Models;
using SimilarityTextComparison.Domain.Models.Matching;
using SimilarityTextComparison.Domain.Models.TextPreProcessing;
using SimilarityTextComparison.Infrastructure.Services;

namespace SimilarityTextComparison.Blazor.Services;

public class ControllerService : IControllerService
{
    private readonly IStorageService _storageService;
    private readonly ITextComparer _simTexter;

    public ControllerService(IStorageService storageService, ITextComparer simTexter)
    {
        _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        _simTexter = simTexter ?? throw new ArgumentNullException(nameof(simTexter));
    }

    public async Task<List<List<MatchSegment>>> CompareTextsAsync(string input1, string input2)
    {
        ValidateInputs(input1, input2);
        List<InputInfo> inputTexts = CreateInputTexts(input1, input2);

        return await ExecuteComparisonAsync(inputTexts);
    }

    private static void ValidateInputs(string input1, string input2)
    {
        if (string.IsNullOrWhiteSpace(input1) || string.IsNullOrWhiteSpace(input2))
        {
            throw new ArgumentException("Uno o entrambi i testi sono vuoti.");
        }
    }

    private static List<InputInfo> CreateInputTexts(string input1, string input2)
    {
        return
        [
            new InputInfo("", "", input1),
            new InputInfo("", "", input2)
        ];
    }

    private async Task<List<List<MatchSegment>>> ExecuteComparisonAsync(List<InputInfo> inputTexts)
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