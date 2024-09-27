namespace ChatGPT_Splitter_Blazor_New.Pages.TextComparer.Model.Comparison;

public class Match
{
    public int SrcTxtIdx { get; }
    public int SrcTkBeginPos { get; }
    public int TrgTxtIdx { get; }
    public int TrgTkBeginPos { get; }
    public int MatchLength { get; }
    public string Value { get; set; }
    public int Index { get; set; }

    public Match(int srcTxtIdx, int srcTkBeginPos, int trgTxtIdx, int trgTkBeginPos, int matchLength)
    {
        SrcTxtIdx = srcTxtIdx;
        SrcTkBeginPos = srcTkBeginPos;
        TrgTxtIdx = trgTxtIdx;
        TrgTkBeginPos = trgTkBeginPos;
        MatchLength = matchLength;
    }
}