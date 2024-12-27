using System.Text.RegularExpressions;

namespace SimilarityTextComparison.Domain.Models.TextPreProcessing;

/// <summary>
/// Represents a text that has been processed (cleaned, tokenized, etc.).
/// </summary>
public class ProcessedText
{
    public string Text { get; set; }
    public TextStatistics Statistics { get; }
    public List<Token> Tokens { get; set; }
    public int TkBeginPos { get; set; }
    public int TkEndPos { get; set; }

    public ProcessedText(string text, TextStatistics statistics, List<Token> tokens)
    {
        Text = text ?? throw new ArgumentNullException(nameof(text));
        Statistics = statistics ?? throw new ArgumentNullException(nameof(statistics));
        Tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
    }


    /// <summary>
    /// Represents statistical information about a text.
    /// </summary>
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
            return Regex.Matches(text, @"\S+").Count;
        }
    }
}