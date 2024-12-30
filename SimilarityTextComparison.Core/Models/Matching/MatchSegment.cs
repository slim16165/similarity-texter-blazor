using SimilarityTextComparison.Domain.Models.Position;
using SimilarityTextComparison.Domain.Models.Position.Enum;
using SimilarityTextComparison.Domain.Models.TextPreProcessing;
using System.Diagnostics;

namespace SimilarityTextComparison.Domain.Models.Matching;

/// <summary>
/// Rappresenta un segmento di testo (match) individuato tra due testi distinti.
/// Un <see cref="MatchSegment"/> specifica:
/// - Indice del testo di riferimento (<see cref="TextIndex"/>),
/// - Posizioni di inizio e fine calcolate in unità di token,
/// - La lunghezza del match in termini di numero di token,
/// - Una classe di stile opzionale per l'evidenziazione (<see cref="StyleClass"/>).
/// </summary>
[DebuggerDisplay("MatchSegment: TextIndex={TextIndex}, Begin={BeginPosition}, Length={Length}, MatchedText={MatchedText}")]
public class MatchSegment : PositionalEntity
{
    /// <summary>
    /// Indice del testo (0, 1, 2, ...) a cui appartiene questo match.
    /// Per esempio:
    /// <list type="bullet">
    /// <item>0 = Testo Sorgente</item>
    /// <item>1 = Testo Target</item>
    /// </list>
    /// </summary>
    public int TextIndex { get; private set; }

    /// <summary>
    /// Posizione di inizio del match in termini di token, equivalente a <see cref="PositionalEntity.BeginPosition"/>.
    /// </summary>
    public int TokenBeginPosition => base.BeginPosition;

    /// <summary>
    /// Lunghezza del match in termini di token.
    /// Aggiorna dinamicamente <see cref="PositionalEntity.EndPosition"/>.
    /// </summary>
    public int MatchLength
    {
        get => base.Length;
        set => EndPosition = BeginPosition + value;
    }

    /// <summary>
    /// Restituisce la posizione finale del token del match (non inclusiva),
    /// ossia <c>TokenBeginPosition + MatchLength</c>.
    /// </summary>
    public int TokenEndPosition => TokenBeginPosition + MatchLength;

    /// <summary>
    /// Testo effettivamente matchato per questo segmento.
    /// Utilizzata esclusivamente per scopi di debug.
    /// </summary>
    public string MatchedText { get; set; } = string.Empty;

    /// <summary>
    /// Classe di stile usata per evidenziare il match (CSS, HTML, ecc.).
    /// </summary>
    public string StyleClass { get; private set; }

    /// <summary>
    /// Costruttore principale di <see cref="MatchSegment"/>.
    /// </summary>
    /// <param name="textIndex">
    /// L'indice del testo (per distinguere se è sorgente o target, se si hanno più testi).
    /// </param>
    /// <param name="tokenBeginPos">
    /// Posizione di inizio (in token) nel testo.
    /// </param>
    /// <param name="matchLength">
    /// Lunghezza del match in termini di token.
    /// </param>
    public MatchSegment(int textIndex, int tokenBeginPos, int matchLength)
        : base(tokenBeginPos, tokenBeginPos + matchLength, PositionUnit.Token)
    {
        TextIndex = textIndex;
        StyleClass = string.Empty; // Di default nessun stile
    }

    /// <summary>
    /// Determina se due segmenti non si sovrappongono.
    /// </summary>
    /// <param name="other">L'altro <see cref="MatchSegment"/> da confrontare.</param>
    /// <returns><c>true</c> se non si sovrappongono, <c>false</c> altrimenti.</returns>
    public bool IsNonOverlapping(MatchSegment other)
    {
        // (TokenEndPosition - 1) < other.TokenBeginPosition => no sovrapposizione
        // e tokenBeginPosition != other.tokenBeginPosition
        return this.TokenBeginPosition != other.TokenBeginPosition
               && (this.TokenEndPosition - 1) < other.TokenBeginPosition;
    }

    /// <summary>
    /// Determina se questo segmento può "estendersi" (o sovrapporsi parzialmente) a un altro,
    /// in base alle posizioni in token.
    /// </summary>
    /// <param name="other">L'altro <see cref="MatchSegment"/> da confrontare.</param>
    /// <returns><c>true</c> se c'è un'estensione/sovrapposizione possibile, <c>false</c> altrimenti.</returns>
    public bool CanExtendOverlap(MatchSegment other)
    {
        return this.TokenEndPosition < other.TokenEndPosition;
    }

    /// <summary>
    /// Fusione o estensione di overlap tra questo segment e un altro: 
    /// può aggiornare la lunghezza di uno dei due segmenti, unire stili, ecc.
    /// Esempio di logica "vuota" da personalizzare.
    /// </summary>
    /// <param name="other">Il <see cref="MatchSegment"/> con cui fare l’"overlap merge".</param>
    public void ExtendOverlap(MatchSegment other)
    {
        if (this.Unit != other.Unit)
            throw new InvalidOperationException("Le unità di posizione non sono coerenti.");

        // Esempio: potresti voler aggiornare la lunghezza se "other" si estende oltre this.
        // Oppure, se la fusione deve avvenire su 'other', aggiungere logica qui.
        // 
        // (Implementazione a piacere, qui ne mettiamo una "neutra"):
        // if (this.CanExtendOverlap(other))
        // {
        //     this.MatchLength = other.TokenEndPosition - this.TokenBeginPosition;
        // }
    }

    /// <summary>
    /// Verifica se la classe di stile attuale contiene già la stringa "overlapping".
    /// </summary>
    public bool ContainsOverlappingStyle()
    {
        return StyleClass.EndsWith(" overlapping");
    }

    /// <summary>
    /// Aggiunge la logica di estensione dello stile in caso di overlap con un altro <see cref="MatchSegment"/>.
    /// </summary>
    /// <param name="other">Il secondo <see cref="MatchSegment"/> che condivide l’overlap.</param>
    public void ExtendOverlapStyle(MatchSegment other)
    {
        if (this.Unit != other.Unit)
            throw new InvalidOperationException("Le unità di posizione non sono coerenti.");

        // Verifica se la classe di stile già contiene "overlapping"
        var newStyle = this.ContainsOverlappingStyle()
            ? this.StyleClass
            : $"{this.StyleClass} overlapping";

        // Applica la nuova classe di stile a entrambi i MatchSegment
        this.SetStyleClass(newStyle);
        other.SetStyleClass(newStyle);

        // Calcolo esemplificativo: se volessi aggiornare la lunghezza di 'other' 
        // basandoti sulle posizioni di 'this':
        // other.MatchLength = other.TokenBeginPosition - this.TokenBeginPosition;
    }

    /// <summary>
    /// Imposta la classe di stile di questo match segment.
    /// </summary>
    /// <param name="styleClass">La classe di stile da applicare.</param>
    public void SetStyleClass(string styleClass)
    {
        StyleClass = styleClass;
    }

    /// <summary>
    /// Imposta la classe di stile di questo match segment,
    /// usando un suffisso numerico (0..9).
    /// </summary>
    /// <param name="styleNumber">Numero usato per generare la classe di stile.</param>
    public void SetStyleClass(int styleNumber)
    {
        SetStyleClass($"hl-{styleNumber % 10}");
    }

    /// <summary>
    /// Crea un link HTML sicuro per evidenziare e collegare questo match segment con un match corrispondente.
    /// </summary>
    /// <param name="text">Il testo (inner text) da rendere link.</param>
    /// <param name="trgMatchSegment">Il match segment di destinazione, a cui collegare il link.</param>
    /// <returns>Una stringa HTML che contiene l'anchor (&lt;a&gt;) con l'ID e l'href appropriati.</returns>
    public string CreateLinkNode(string text, MatchSegment trgMatchSegment)
    {
        var matchLinkId = $"{TextIndex + 1}-{TokenBeginPosition}";
        var href = $"#{trgMatchSegment.TextIndex + 1}-{trgMatchSegment.TokenBeginPosition}";
        var safeText = System.Net.WebUtility.HtmlEncode(text);

        return $"<a id='{matchLinkId}' " +
               $"class='{System.Net.WebUtility.HtmlEncode(StyleClass)}' " +
               $"href='{System.Net.WebUtility.HtmlEncode(href)}'>" +
               $"{safeText}</a>";
    }

    /// <summary>
    /// Restituisce la posizione di inizio nel testo originale, sfruttando le coordinate di <see cref="Token"/>.
    /// </summary>
    /// <param name="tokens">Lista dei token di riferimento.</param>
    /// <returns>Posizione iniziale del match in termini di caratteri.</returns>
    public int GetTxtBeginPos(List<Token> tokens)
    {
        return tokens[TokenBeginPosition].BeginPosition;
    }

    /// <summary>
    /// Restituisce la posizione di fine nel testo originale, sfruttando le coordinate di <see cref="Token"/>.
    /// </summary>
    /// <param name="tokens">Lista dei token di riferimento.</param>
    /// <returns>Posizione finale (non inclusiva) del match in termini di caratteri.</returns>
    public int GetTxtEndPos(List<Token> tokens)
    {
        return tokens[TokenBeginPosition + MatchLength - 1].EndPosition;
    }

    /// <summary>
    /// Sovrascrive il metodo ToString per includere informazioni aggiuntive.
    /// </summary>
    public override string ToString()
    {
        return $"MatchSegment: TextIndex={TextIndex}, Begin={BeginPosition}, Length={Length}, MatchedText='{MatchedText}', StyleClass='{StyleClass}'";
    }

    /// <summary>
    /// Recupera il testo matchato dato un elenco di token.
    /// </summary>
    /// <param name="tokens">Lista di token del testo originale.</param>
    /// <returns>Stringa contenente il testo matchato.</returns>
    public string RetrieveMatchedText(List<Token> tokens)
    {
        if (tokens == null)
            throw new ArgumentNullException(nameof(tokens));

        if (BeginPosition < 0 || BeginPosition + Length > tokens.Count)
            throw new ArgumentOutOfRangeException("Le posizioni del match segment sono fuori dai limiti dei token.");

        return string.Join(" ", tokens.Skip(BeginPosition).Take(Length).Select(t => t.Text));
    }
}