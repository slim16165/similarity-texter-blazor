using SimilarityTextComparison.Domain.Models.Position.Enum;

namespace SimilarityTextComparison.Domain.Models.Position;

/// <summary>
/// Represents a generic positional entity in a text, with a defined start, end, and unit of measurement.
/// </summary>
public abstract class PositionalEntity
{
    public int BeginPosition { get; protected set; }
    public int EndPosition { get; protected set; }
    public PositionUnit Unit { get; protected set; }

    protected PositionalEntity(int beginPosition, int endPosition, PositionUnit unit)
    {
        if (endPosition < beginPosition)
            throw new ArgumentException("EndPosition deve essere maggiore o uguale a BeginPosition.");

        BeginPosition = beginPosition;
        EndPosition = endPosition;
        Unit = unit;
    }

    public int Length => EndPosition - BeginPosition;
}