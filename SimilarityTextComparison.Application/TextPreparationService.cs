using SimilarityTextComparison.Application.Pipeline;
using SimilarityTextComparison.Domain.Interfaces.TextProcessing;
using SimilarityTextComparison.Domain.Models.TextPreProcessing;

namespace SimilarityTextComparison.Application;

public class TextPreparationService : ITextPreparationService
{
    private readonly MatchingPipeline _matchingPipeline;

    public TextPreparationService(MatchingPipeline matchingPipeline)
    {
        _matchingPipeline = matchingPipeline;
    }

    public async Task<(ProcessedText processedText, List<Token> tokens)> PreProcessAndTokenizeText(string inputText)
    {
        var context = new MatchingContext
        {
            SourceText = new ProcessedText(inputText, new ProcessedText.TextStatistics(inputText), new List<Token>()),
            TargetText = new ProcessedText(string.Empty, new ProcessedText.TextStatistics(string.Empty), new List<Token>())
        };

        await _matchingPipeline.ExecuteAsync(context);

        return (context.SourceText, context.SourceText.Tokens);
    }
}