namespace ChatGPT_Splitter_Blazor_New.TextComparer.Model;

public class PositionalEntity
{
    public int BeginPosition { get; protected set; }
    public int EndPosition { get; protected set; }
    public PositionUnit Unit { get; protected set; }

    public PositionalEntity(int beginPosition, int endPosition, PositionUnit unit)
    {
        if (endPosition < beginPosition)
            throw new ArgumentException("EndPosition deve essere maggiore o uguale a BeginPosition.");

        BeginPosition = beginPosition;
        EndPosition = endPosition;
        Unit = unit;
    }

    public int Length => EndPosition - BeginPosition;
}

public class IndexedPositionalEntity : PositionalEntity
{
    public int TextIndex { get; private set; }

    public IndexedPositionalEntity(int textIndex, int beginPosition, int endPosition, PositionUnit unit)
        : base(beginPosition, endPosition, unit)
    {
        TextIndex = textIndex;
    }
}