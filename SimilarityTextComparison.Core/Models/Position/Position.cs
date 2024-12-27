using SimilarityTextComparison.Domain.Models.Position.Enum;

namespace SimilarityTextComparison.Domain.Models.Position;

/// <summary>
/// Represents a position within a text, either by character or by token.
/// </summary>
public class Position : PositionalEntity
{
    public int TextIndex { get; private set; }

    public Position(int beginPosition, int endPosition, PositionUnit unit, int textIndex = 0)
        : base(beginPosition, endPosition, unit)
    {
        TextIndex = textIndex;
    }

    public bool Contains(int position)
    {
        return position >= BeginPosition && position < EndPosition;
    }

    public bool Overlaps(Position other)
    {
        if (Unit != other.Unit)
            throw new InvalidOperationException("Cannot compare positions with different units.");

        return BeginPosition < other.EndPosition && other.BeginPosition < EndPosition;
    }
}