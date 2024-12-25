using System.Text.RegularExpressions;
using SimilarityTextComparison.Core.Models.TextProcessing;

namespace SimilarityTextComparison.Core.Services.TextProcessing;

public class Tokenizer
{
    private readonly Configuration.Configuration _config;
    public static List<Token> GlobalTokens;

    public Tokenizer(Configuration.Configuration config)
    {
        _config = config;
    }

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
    public List<Token> Tokenize(string text)
    {
        var tokens = new List<Token>();
        var matches = Regex.Matches(text, @"\S+");

        foreach (Match match in matches)
        {
            var cleanedWord = ReplaceUmlauts(match.Value);

            if (!string.IsNullOrEmpty(cleanedWord))
            {
                tokens.Add(new Token(text: cleanedWord, textBeginPos: match.Index));
            }
        }

        GlobalTokens = tokens;

        return tokens;
    }

    private string ReplaceUmlauts(string word)
    {
        if (!_config.ReplaceUmlaut) return word;

        return word.Replace("ä", "ae")
            .Replace("ö", "oe")
            .Replace("ü", "ue")
            .Replace("ß", "ss")
            .Replace("Ä", "AE")
            .Replace("Ö", "OE")
            .Replace("Ü", "UE");
    }
}