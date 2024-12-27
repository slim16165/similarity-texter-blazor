using SimilarityTextComparison.Application.Interfaces;
using SimilarityTextComparison.Domain.Models.Matching;
using SimilarityTextComparison.Domain.Models.TextPreProcessing;
using SimilarityTextComparison.Infrastructure.Services;

namespace SimilarityTextComparison.Blazor.Services;

public class TextComparisonService : IControllerService
{
    private readonly IStorageService _storageService;
    private readonly ITextComparer _textComparer;

    public TextComparisonService(IStorageService storageService, ITextComparer textComparer)
    {
        _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        _textComparer = textComparer ?? throw new ArgumentNullException(nameof(textComparer));
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
            throw new ArgumentException("Entrambi i testi devono essere forniti e non vuoti.");
        }
    }

    private static List<InputInfo> CreateInputTexts(string input1, string input2)
    {
        return new List<InputInfo>
        {
            new InputInfo("Source", "File1", input1),
            new InputInfo("Target", "File2", input2)
        };
    }

    private async Task<List<List<MatchSegment>>> ExecuteComparisonAsync(List<InputInfo> inputTexts)
    {
        try
        {
            return await _textComparer.CompareAsync(inputTexts);
        }
        catch (Exception ex)
        {
            // Log dell'errore se necessario
            throw new Exception($"Errore durante il confronto: {ex.Message}", ex);
        }
    }
}