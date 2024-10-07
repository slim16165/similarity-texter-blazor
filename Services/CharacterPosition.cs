using ChatGPT_Splitter_Blazor_New.TextComparer.Model;

namespace ChatGPT_Splitter_Blazor_New.TextComparer.Services;

public class CharacterPosition : PositionalEntity
{
    public CharacterPosition(int beginCharPos, int endCharPos)
        : base(beginCharPos, endCharPos, PositionUnit.Character)
    {
    }

    public bool Contains(int charPosition)
    {
        return charPosition >= BeginPosition && charPosition < EndPosition;
    }
}