using SimilarityTextComparison.Domain.Models.Position;
using SimilarityTextComparison.Domain.Models.Position.Enum;
using SimilarityTextComparison.Domain.Models.TextPreProcessing;

namespace SimilarityTextComparison.Domain.Models.Matching;

/// <summary>
/// Represents a matched segment between two texts.
/// </summary>
public class MatchSegment : PositionalEntity
{
    public int TextIndex { get; private set; }
    public int TokenBeginPosition => base.BeginPosition;
    public int MatchLength
    {
        get => base.Length;
        set => EndPosition = BeginPosition + value;
    }

    public string StyleClass { get; set; }

    public MatchSegment(int textIndex, int tokenBeginPos, int matchLength)
        : base(tokenBeginPos, tokenBeginPos + matchLength, PositionUnit.Token)
    {
        TextIndex = textIndex;
        StyleClass = string.Empty;
    }

    /// <summary>
    /// Crea il link associato al match in modo sicuro.
    /// </summary>
    /// <param name="text">Il contenuto del nodo.</param>
    /// <param name="trgMatchSegment">Il match segment di destinazione.</param>
    /// <returns>Il nodo HTML come stringa con link.</returns>
    public string CreateLinkNode(string text, MatchSegment trgMatchSegment)
    {
        var matchLinkId = $"{TextIndex + 1}-{TokenBeginPosition}";
        var href = $"#{trgMatchSegment.TextIndex + 1}-{trgMatchSegment.TokenBeginPosition}";
        var safeText = System.Net.WebUtility.HtmlEncode(text);
        return $"<a id='{matchLinkId}' class='{System.Net.WebUtility.HtmlEncode(StyleClass)}' href='{System.Net.WebUtility.HtmlEncode(href)}'>{safeText}</a>";
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
        return tokens[TokenBeginPosition].BeginPosition;
    }

    /// <summary>
    /// Restituisce la posizione finale del testo del match.
    /// </summary>
    /// <param name="tokens">Lista di token con posizione del testo.</param>
    /// <returns>Posizione finale del match nel testo.</returns>
    public int GetTxtEndPos(List<Token> tokens)
    {
        return tokens[TokenBeginPosition + MatchLength - 1].EndPosition;
    }

    /// <summary>
    /// Imposta la classe di stile del match segment.
    /// </summary>
    /// <param name="styleClass">La classe di stile da applicare.</param>
    public void SetStyleClass(string styleClass)
    {
        StyleClass = styleClass;
    }

    public void SetStyleClass(int styleNumber)
    {
        SetStyleClass($"hl-{styleNumber % 10}");
    }
}