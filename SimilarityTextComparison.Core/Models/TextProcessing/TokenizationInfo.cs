namespace SimilarityTextComparison.Core.Models.TextProcessing;

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