using SimilarityTextComparison.Core.Models.Position;

namespace SimilarityTextComparison.Core.Models.TextProcessing;

public class ProcessedText : CharacterPosition
{
    public string Text { get; }  
    public InputInfo InputInformation { get; }
    public TextStatistics Statistics { get; }
    public TokenizationInfo Tokenization { get; }

    public ProcessedText(string text, InputInfo inputInfo, TextStatistics statistics, TokenizationInfo tokenization)
        : base(tokenization.TokenBeginPosition, tokenization.TokenEndPosition)
    {
        Text = text;  // Assegna il testo alla proprietà Text
        InputInformation = inputInfo;
        Statistics = statistics;
        Tokenization = tokenization;
    }
}