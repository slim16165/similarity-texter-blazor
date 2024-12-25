using SimilarityTextComparison.Domain.Models.Position;

namespace SimilarityTextComparison.Domain.Models.Comparison;

public class MyMatch
{
    public int SourceTextIndex { get; }
    public int TargetTextIndex { get; }

    public IndexedPositionalEntity SourcePosition { get; }
    public IndexedPositionalEntity TargetPosition { get; }

    public int SourceTokenkBeginPos => SourcePosition.BeginPosition;
    public int TargetTokenBeginPos => TargetPosition.BeginPosition;
    public int MatchLength => SourcePosition.Length;

    //public MyMatch(
    //    int sourceTextIndex,
    //    PositionalEntity sourcePosition,
    //    int targetTextIndex,
    //    PositionalEntity targetPosition)
    //{
    //    SourceTextIndex = sourceTextIndex;
    //    SourcePosition = sourcePosition ?? throw new ArgumentNullException(nameof(sourcePosition));
    //    TargetTextIndex = targetTextIndex;
    //    TargetPosition = targetPosition ?? throw new ArgumentNullException(nameof(targetPosition));
    //}
}