namespace SimilarityTextComparison.Domain.Models.Position;

public class TokenPosition : PositionalEntity
{
    public int BeginPositionInToken => BeginPosition;
    public int EndPositionInToken => EndPosition;

    public TokenPosition(int beginTokenPos, int endTokenPos)
        : base(beginTokenPos, endTokenPos, PositionUnit.Token)
    {
    }

    public bool Overlaps(TokenPosition other)
    {
        return BeginPosition < other.EndPosition && other.BeginPosition < EndPosition;
    }
}