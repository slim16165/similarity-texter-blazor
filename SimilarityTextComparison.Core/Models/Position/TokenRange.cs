using SimilarityTextComparison.Domain.Models.Position.Enum;

namespace SimilarityTextComparison.Domain.Models.Position;

/// <summary>
/// Represents a range of tokens within a text.
/// </summary>
public class TokenRange : PositionalEntity
{
    public int TokenCount => Length;

    public TokenRange(int tokenBeginPosition, int tokenEndPosition)
        : base(tokenBeginPosition, tokenEndPosition, PositionUnit.Token)
    {
    }
}