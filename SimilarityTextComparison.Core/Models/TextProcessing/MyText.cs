using System.Text.RegularExpressions;
using SimilarityTextComparison.Core.Models.Position;

namespace SimilarityTextComparison.Core.Models.TextProcessing;

public class MyText : CharacterPosition
{
    public string Text { get; }  
    public InputInfo InputInformation { get; }
    public TextStatistics Statistics { get; }
    public TokenizationInfo Tokenization { get; }

    public MyText(string text, InputInfo inputInfo, TextStatistics statistics, TokenizationInfo tokenization)
        : base(tokenization.TokenBeginPosition, tokenization.TokenEndPosition)
    {
        Text = text;  // Assegna il testo alla proprietà Text
        InputInformation = inputInfo;
        Statistics = statistics;
        Tokenization = tokenization;
    }
}

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

public class TokenizationInfo
{
    public int TokenBeginPosition { get; }
    public int TokenEndPosition { get; }

    public TokenizationInfo(int beginPosition, int tokenCount)
    {
        TokenBeginPosition = beginPosition;
        TokenEndPosition = beginPosition + tokenCount;
    }
}

public class InputInfo
{
    public string InputMode { get; }
    public string FileName { get; }

    public InputInfo(string inputMode, string fileName)
    {
        InputMode = inputMode;
        FileName = fileName;
    }
}