using SimilarityTextComparison.Domain.Models.Position.Enum;
using SimilarityTextComparison.Domain.Models.TextPreProcessing;

namespace SimilarityTextComparison.Domain.Models.Position;

/// <summary>
/// Represents a position based on tokens.
/// </summary>
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

public static class TokenPositionExtensions
{
    public static string RetrieveMatchedText(this TokenPosition pos, List<Token> tokens)
    {
        if (tokens == null)
            throw new ArgumentNullException(nameof(tokens));

        // Controllo di sicurezza
        if (pos.BeginPosition < 0 || pos.EndPosition > tokens.Count)
            throw new ArgumentOutOfRangeException(
                $"Posizioni non valide: Begin={pos.BeginPosition}, End={pos.EndPosition}, TokenCount={tokens.Count}");

        int length = pos.Length; // endPos - beginPos
        return string.Join(" ", tokens
            .Skip(pos.BeginPosition)
            .Take(length)
            .Select(t => t.Text));
    }
}
