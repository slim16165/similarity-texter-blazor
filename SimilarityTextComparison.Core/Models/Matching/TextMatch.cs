using SimilarityTextComparison.Domain.Models.Position;

namespace SimilarityTextComparison.Domain.Models.Matching;

/// <summary>
/// Represents a match between a source text and a target text.
/// </summary>
public class TextMatch
{
    public int SourceTextIndex { get; }
    public int TargetTextIndex { get; }

    public TokenPosition SourcePosition { get; }
    public TokenPosition TargetPosition { get; }

    public int SourceTokenBeginPos => SourcePosition.BeginPosition;
    public int TargetTokenBeginPos => TargetPosition.BeginPosition;
    public int MatchLength => SourcePosition.Length;

    public TextMatch(int sourceTextIndex, TokenPosition sourcePosition, int targetTextIndex, TokenPosition targetPosition)
    {
        SourceTextIndex = sourceTextIndex;
        SourcePosition = sourcePosition ?? throw new ArgumentNullException(nameof(sourcePosition));
        TargetTextIndex = targetTextIndex;
        TargetPosition = targetPosition ?? throw new ArgumentNullException(nameof(targetPosition));
    }
}