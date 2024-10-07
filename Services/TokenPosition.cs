using ChatGPT_Splitter_Blazor_New.TextComparer.Model;

namespace ChatGPT_Splitter_Blazor_New.TextComparer.Services;

public class TokenPosition : PositionalEntity
{
    public TokenPosition(int beginTokenPos, int endTokenPos)
        : base(beginTokenPos, endTokenPos, PositionUnit.Token)
    {
    }

    public bool Overlaps(TokenPosition other)
    {
        return BeginPosition < other.EndPosition && other.BeginPosition < EndPosition;
    }
}