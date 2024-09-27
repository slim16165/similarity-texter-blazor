using ChatGPT_Splitter_Blazor_New.Pages.TextComparer.Model.TextProcessing;

namespace ChatGPT_Splitter_Blazor_New.Pages.TextComparer.Model.Comparison;

public class MatchSegment
{
    public int TxtIdx { get; }
    public int TkBeginPos { get; }
    public int MatchLength { get; }
    public string StyleClass { get; private set; }

    public MatchSegment(int txtIdx, int tkBeginPos, int matchLength)
    {
        TxtIdx = txtIdx;
        TkBeginPos = tkBeginPos;
        MatchLength = matchLength;
        StyleClass = string.Empty;
    }

    /// <summary>
    /// Crea il link associato al match.
    /// </summary>
    /// <param name="text">Il contenuto del nodo.</param>
    /// <param name="trgMatchSegment">Il match segment di destinazione.</param>
    /// <returns>Il nodo HTML come stringa con link.</returns>
    public string CreateLinkNode(string text, MatchSegment trgMatchSegment)
    {
        var matchLinkId = $"{TxtIdx + 1}-{TkBeginPos}";
        var href = $"#{trgMatchSegment.TxtIdx + 1}-{trgMatchSegment.TkBeginPos}";
        return $"<a id='{matchLinkId}' class='{StyleClass}' href='{href}'>{text}</a>";
    }

    /// <summary>
    /// Restituisce la posizione finale del token del match.
    /// </summary>
    /// <returns>La posizione finale del token (non inclusivo).</returns>
    public int GetTkEndPosition()
    {
        return TkBeginPos + MatchLength;
    }

    /// <summary>
    /// Restituisce la posizione di inizio del testo del match.
    /// </summary>
    /// <param name="tokens">Lista di token con posizione del testo.</param>
    /// <returns>Posizione iniziale del match nel testo.</returns>
    public int GetTxtBeginPos(List<Token> tokens)
    {
        return tokens[TkBeginPos].TxtBeginPos;
    }

    /// <summary>
    /// Restituisce la posizione finale del testo del match.
    /// </summary>
    /// <param name="tokens">Lista di token con posizione del testo.</param>
    /// <returns>Posizione finale del match nel testo.</returns>
    public int GetTxtEndPos(List<Token> tokens)
    {
        return tokens[TkBeginPos + MatchLength - 1].TxtEndPos;
    }

    /// <summary>
    /// Imposta la classe di stile del match segment.
    /// </summary>
    /// <param name="n">La classe di stile da applicare.</param>
    public void SetStyleClass(object n)
    {
        if (n is int)
        {
            StyleClass = $"hl-{(int)n % 10}";
        }
        else if (n is string)
        {
            StyleClass = n.ToString();
        }
    }
}