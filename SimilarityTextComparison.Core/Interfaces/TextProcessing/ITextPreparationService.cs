using SimilarityTextComparison.Domain.Models.TextPreProcessing;

namespace SimilarityTextComparison.Domain.Interfaces.TextProcessing;

public interface ITextPreparationService
{
    /// <summary>
    /// Prepara il testo dall'input HTML o testo semplice.
    /// </summary>
    /// <param name="inputText">L'input di testo o HTML.</param>
    /// <returns>Lista di token processati.</returns>
    Task<(ProcessedText processedText, List<Token> tokens)> PreProcessAndTokenizeText(string inputText);
}