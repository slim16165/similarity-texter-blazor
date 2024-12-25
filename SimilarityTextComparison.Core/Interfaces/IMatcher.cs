using SimilarityTextComparison.Domain.Models.Comparison;
using SimilarityTextComparison.Domain.Models.TextProcessing;

namespace SimilarityTextComparison.Domain.Interfaces;

public interface IMatcher
{
    /// <summary>
    /// Trova i segmenti corrispondenti tra il testo sorgente e il testo di destinazione.
    /// </summary>
    /// <param name="sourceTextIndex">Indice del testo sorgente.</param>
    /// <param name="targetTextIndex">Indice del testo di destinazione.</param>
    /// <param name="sourceText">Il testo sorgente.</param>
    /// <param name="targetText">Il testo di destinazione.</param>
    /// <param name="forwardReferences">I riferimenti avanzati per il testo sorgente.</param>
    /// <param name="tokens">La lista di tutti i token.</param>
    /// <returns>Una lista di liste di segmenti corrispondenti.</returns>
    List<List<MatchSegment>> FindMatches(
        int sourceTextIndex,
        int targetTextIndex,
        ProcessedText sourceText,
        ProcessedText targetText,
        Dictionary<int, int> forwardReferences,
        List<Token> tokens);
}