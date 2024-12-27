using SimilarityTextComparison.Application.Interfaces;
using SimilarityTextComparison.Domain.Models.Matching;
using SimilarityTextComparison.Domain.Models.TextPreProcessing;

namespace SimilarityTextComparison.Blazor.Services;

public class TextComparisonService : IControllerService
{
    private readonly ITextComparer _textComparer;

    public TextComparisonService(ITextComparer textComparer)
    {
        _textComparer = textComparer ?? throw new ArgumentNullException(nameof(textComparer));
    }

    public async Task<List<List<MatchSegment>>> CompareTextsAsync(string input1, string input2)
    {
        ValidateInputs(input1, input2);
        List<InputInfo> inputTexts = CreateInputTexts(input1, input2);

        return await _textComparer.CompareAsync(inputTexts);
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
            new InputInfo("Source", "Text1", input1),
            new InputInfo("Target", "Text2", input2)
        };
    }
}