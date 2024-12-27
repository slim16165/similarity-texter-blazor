using System.Text.RegularExpressions;
using SimilarityTextComparison.Domain.Interfaces.TextProcessing;
using SimilarityTextComparison.Domain.Models.TextPreProcessing;
using SimilarityTextComparison.Infrastructure.Services;

namespace SimilarityTextComparison.Domain.Services.TextPreProcessing;

/// <summary>
/// Suddivide il testo in token (parole) con informazioni di posizione.
/// </summary>
public class Tokenizer : ITokenizer
{
    private readonly TextComparisonConfiguration _config;

    public Tokenizer(TextComparisonConfiguration config)
    {
        _config = config;
    }

    /// <summary>
    /// Suddivide un testo in una lista di token, dove ciascun token rappresenta una parola.
    /// </summary>
    /// <returns>
    /// Una lista di oggetti <see cref="Token"/>. Ogni token rappresenta una parola "pulita" dal testo originale.
    /// Il token contiene:
    /// - La parola "pulita" dopo la sostituzione di eventuali caratteri speciali
    /// - La posizione iniziale della parola nel testo originale.
    /// - La posizione finale della parola (calcolata internamente in base alla lunghezza della parola).
    /// Se una parola risulta vuota dopo la pulizia, non viene aggiunta alla lista.
    /// </returns>
    public List<Token> Tokenize(string text)
    {
        var tokens = new List<Token>();
        var matches = Regex.Matches(text, @"\S+");

        foreach (Match match in matches)
        {
            var cleanedWord = match.Value;

            if (!string.IsNullOrEmpty(cleanedWord))
            {
                tokens.Add(new Token(
                    text: cleanedWord,
                    beginPosition: match.Index,
                    endPosition: match.Index + cleanedWord.Length
                ));
            }
        }

        return tokens;
    }
}