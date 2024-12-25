using SimilarityTextComparison.Domain.Models.TextProcessing;

namespace SimilarityTextComparison.Domain.Interfaces;

public interface ITextPreparationService
{
    /// <summary>
    /// Prepara il testo dall'input HTML o testo semplice.
    /// </summary>
    /// <param name="inputText">L'input di testo o HTML.</param>
    /// <returns>Lista di token processati.</returns>
    Task<List<Token>> PrepareTextAsync(string inputText);
}