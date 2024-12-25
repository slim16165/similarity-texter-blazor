namespace SimilarityTextComparison.Domain.Models.Position;

public class IndexedPositionalEntity : TokenPosition
{
    public int TextIndex { get; private set; }

    public IndexedPositionalEntity(int textIndex, int beginPosition, int endPosition)
        : base(beginPosition, endPosition)
    {
        TextIndex = textIndex;
    }
}