using SimilarityTextComparison.Domain.Models.Position;
using SimilarityTextComparison.Domain.Models.Position.Enum;

namespace SimilarityTextComparison.Domain.Models.TextPreProcessing;

/// <summary>
/// Represents a token (word) in a text with its position.
/// </summary>
public class Token : PositionalEntity
{
    public string Text { get; }


    public Token(string text, int beginPosition, int endPosition)
        : base(beginPosition, endPosition, PositionUnit.Character)
    {
        Text = text ?? throw new ArgumentNullException(nameof(text));
    }

    public static Token FromRange(string text, TokenRange range)
    {
        return new Token(text, range.BeginPosition, range.EndPosition);
    }
}