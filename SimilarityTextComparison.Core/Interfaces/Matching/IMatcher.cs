using SimilarityTextComparison.Domain.Models.Matching;
using SimilarityTextComparison.Domain.Models.TextPreProcessing;

namespace SimilarityTextComparison.Domain.Interfaces.Matching;

public interface IMatcher
{
    /// <summary>
    /// Trova i segmenti corrispondenti tra il testo sorgente e il testo di destinazione.
    /// </summary>
    /// <param name="sourceTextIndex">Indice del testo sorgente.</param>
    /// <param name="targetTextIndex">Indice del testo di destinazione.</param>
    /// <param name="sourceText">Il testo sorgente.</param>
    /// <param name="targetText">Il testo di destinazione.</param>
    /// <param name="unifiedForwardReferences">La lista globale di forward references.</param>
    /// <param name="unifiedTokens">La lista di tutti i token.</param>
    /// <returns>Una lista di liste di segmenti corrispondenti.</returns>
    public List<List<MatchSegment>> FindMatches(
        int sourceTextIndex,
        int targetTextIndex,
        ProcessedText sourceText,
        ProcessedText targetText,
        List<ForwardReference> unifiedForwardReferences,
        List<Token> unifiedTokens);
}