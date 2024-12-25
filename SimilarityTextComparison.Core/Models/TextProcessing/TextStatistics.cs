using System.Text.RegularExpressions;

namespace SimilarityTextComparison.Domain.Models.TextProcessing;

public class TextStatistics
{
    public int NumberOfCharacters { get; }
    public int NumberOfWords { get; }

    public TextStatistics(string text)
    {
        NumberOfCharacters = text.Length;
        NumberOfWords = CountWords(text);
    }

    private static int CountWords(string text)
    {
        // Conta le parole in base a spazi e caratteri non vuoti
        return Regex.Matches(text, @"\S+").Count;
    }
}