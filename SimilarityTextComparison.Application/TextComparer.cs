using SimilarityTextComparison.Application.Interfaces;
using SimilarityTextComparison.Application.Pipeline;
using SimilarityTextComparison.Domain.Models.Matching;
using SimilarityTextComparison.Domain.Models.TextPreProcessing;

namespace SimilarityTextComparison.Application;
public class TextComparer : ITextComparer
{
    private readonly MatchingPipeline _matchingPipeline;

    public TextComparer(MatchingPipeline matchingPipeline)
    {
        _matchingPipeline = matchingPipeline ?? throw new ArgumentNullException(nameof(matchingPipeline));
    }

    /// <summary>
    /// Confronta una lista di testi di input e restituisce i segmenti corrispondenti trovati.
    /// </summary>
    public async Task<List<List<MatchSegment>>> CompareAsync(List<InputInfo> inputTexts)
    {
        if (inputTexts == null || inputTexts.Count < 2)
            throw new ArgumentException("Sono necessari almeno due testi per il confronto.", nameof(inputTexts));

        var context = new MatchingContext
        {
            SourceText = new ProcessedText(inputTexts[0].Text, new ProcessedText.TextStatistics(inputTexts[0].Text), new List<Token>()),
            TargetText = new ProcessedText(inputTexts[1].Text, new ProcessedText.TextStatistics(inputTexts[1].Text), new List<Token>())
        };

        await _matchingPipeline.ExecuteAsync(context);

        if (context.MatchingSegments.Count == 0)
            throw new InvalidOperationException("Nessuna similarità trovata.");

        return context.MatchingSegments;
    }

    /// <summary>
    /// Overload del metodo CompareAsync per confrontare esattamente due testi.
    /// </summary>
    public async Task<List<List<MatchSegment>>> CompareAsync(string text1, string text2)
    {
        if (string.IsNullOrWhiteSpace(text1))
            throw new ArgumentException("Il primo testo non può essere nullo o vuoto.", nameof(text1));

        if (string.IsNullOrWhiteSpace(text2))
            throw new ArgumentException("Il secondo testo non può essere nullo o vuoto.", nameof(text2));

        var inputTexts = new List<InputInfo>
        {
            new InputInfo("Source", "Text1", text1),
            new InputInfo("Target", "Text2", text2)
        };

        return await CompareAsync(inputTexts);
    }
}