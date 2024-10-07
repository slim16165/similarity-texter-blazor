using ChatGPT_Splitter_Blazor_New.TextComparer.Model.TextProcessing;

namespace ChatGPT_Splitter_Blazor_New.TextComparer.Model.Comparison;

public class MatchSegment : IndexedPositionalEntity
{
    //public int TextIndex => base.TextIndex;
    public int TokenBeginPosition => base.BeginPosition;
    public int MatchLength
    {
        get => base.Length;
        set => throw new NotImplementedException();
    }

    public string StyleClass { get; private set; }

    public MatchSegment(int textIndex, PositionalEntity position) : base(textIndex, position.BeginPosition, position.EndPosition, PositionUnit.Token)
    {
        StyleClass = string.Empty;
    }

    public MatchSegment(int textIndex, int tokenkBeginPos, int matchLength) : this(textIndex, new PositionalEntity(tokenkBeginPos, tokenkBeginPos + matchLength, PositionUnit.Token))
    {
    }

    /// <summary>
    /// Crea il link associato al match.
    /// </summary>
    /// <param name="text">Il contenuto del nodo.</param>
    /// <param name="trgMatchSegment">Il match segment di destinazione.</param>
    /// <returns>Il nodo HTML come stringa con link.</returns>
    public string CreateLinkNode(string text, MatchSegment trgMatchSegment)
    {
        var matchLinkId = $"{TextIndex + 1}-{TokenBeginPosition}";
        var href = $"#{trgMatchSegment.TextIndex + 1}-{trgMatchSegment.TokenBeginPosition}";
        return $"<a id='{matchLinkId}' class='{StyleClass}' href='{href}'>{text}</a>";
    }

    /// <summary>
    /// Restituisce la posizione finale del token del match.
    /// </summary>
    /// <returns>La posizione finale del token (non inclusivo).</returns>
    public int GetTkEndPosition()
    {
        return TokenBeginPosition + MatchLength;
    }

    /// <summary>
    /// Restituisce la posizione di inizio del testo del match.
    /// </summary>
    /// <param name="tokens">Lista di token con posizione del testo.</param>
    /// <returns>Posizione iniziale del match nel testo.</returns>
    public int GetTxtBeginPos(List<Token> tokens)
    {
        return tokens[TokenBeginPosition].TextBeginPos;
    }

    /// <summary>
    /// Restituisce la posizione finale del testo del match.
    /// </summary>
    /// <param name="tokens">Lista di token con posizione del testo.</param>
    /// <returns>Posizione finale del match nel testo.</returns>
    public int GetTxtEndPos(List<Token> tokens)
    {
        return tokens[TokenBeginPosition + MatchLength - 1].TextEndPos;
    }

    /// <summary>
    /// Imposta la classe di stile del match segment.
    /// </summary>
    /// <param name="n">La classe di stile da applicare.</param>
    public void SetStyleClass(string styleClass)
    {
        StyleClass = styleClass;
    }

    public void SetStyleClass(int styleNumber)
    {
        SetStyleClass($"hl-{styleNumber % 10}");
    }
}
