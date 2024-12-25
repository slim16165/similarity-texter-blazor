using SimilarityTextComparison.Domain.Models.TextProcessing;

namespace SimilarityTextComparison.Domain.Interfaces;

public interface ITokenizer
{
    /// <summary>
    /// Suddivide un testo in una lista di token, dove ciascun token rappresenta una parola.
    /// </summary>
    /// <returns>
    /// Una lista di oggetti <see cref="Token"/>. Ogni token rappresenta una parola "pulita" dal testo originale.
    /// Il token contiene:
    /// - La parola "pulita" dopo la sostituzione di eventuali caratteri speciali (come umlauti).
    /// - La posizione iniziale della parola nel testo originale.
    /// - La posizione finale della parola (calcolata internamente in base alla lunghezza della parola).
    /// Se una parola risulta vuota dopo la pulizia, non viene aggiunta alla lista.
    /// </returns>
    List<Token> Tokenize(string text);
}