using SimilarityTextComparison.Domain.Interfaces.Styling;
using SimilarityTextComparison.Domain.Models.Matching;

namespace SimilarityTextComparison.Domain.Services.Styling;

/// <summary>
/// Classe responsabile di applicare stili (classi CSS, ad esempio)
/// ai segmenti di match, gestendo eventuali sovrapposizioni.
/// </summary>
public class StyleApplier : IStyleApplier
{
    /// <summary>
    /// Numero di match unici a cui è stato applicato uno stile.
    /// </summary>
    public int UniqueMatches { get; private set; }

    /// <summary>
    /// Applica stili a una lista di liste di <see cref="MatchSegment"/>,
    /// gestendo sovrapposizioni e contatori di stile.
    /// </summary>
    /// <param name="matches">Lista di match, ognuno composto da coppie (o più segmenti).</param>
    /// <returns>Una lista di match "stile-applied".</returns>
    public List<List<MatchSegment>> ApplyStyles(List<List<MatchSegment>> matches)
    {
        var styleClassCount = 1;
        var uniqueMatches = new List<List<MatchSegment>>();

        // itera su ogni coppia di match
        foreach (var matchPair in matches)
        {
            AssignStyleToMatch(matchPair, uniqueMatches, ref styleClassCount);
        }

        UniqueMatches = uniqueMatches.Count;
        return uniqueMatches;
    }

    /// <summary>
    /// Metodo principale che gestisce l'applicazione dello stile a un nuovo match,
    /// controllando se è non-sovrapposto, se può estendersi, ecc.
    /// </summary>
    private static void AssignStyleToMatch(
        List<MatchSegment> currentMatch,
        List<List<MatchSegment>> uniqueMatches,
        ref int styleClassCount)
    {
        // Se è il primo match, assegna e vai
        if (!uniqueMatches.Any())
        {
            InitializeFirstMatchStyle(currentMatch, uniqueMatches);
            return;
        }

        // Prendiamo l'ultimo match nella lista e confrontiamo i segmenti target (index = 1)
        var lastUniqueMatch = uniqueMatches.Last()[1];
        var current = currentMatch[1];

        // Se non si sovrappongono, applichiamo uno stile nuovo
        if (lastUniqueMatch.IsNonOverlapping(current))
        {
            ApplyNewStyle(currentMatch, styleClassCount, uniqueMatches);
            styleClassCount++;
        }
        // Se si possono estendere/overlappare
        else if (lastUniqueMatch.CanExtendOverlap(current))
        {
            // Semplice estensione dello stile di overlap
            lastUniqueMatch.ExtendOverlapStyle(current);
            // Aggiungiamo comunque la coppia ai match
            uniqueMatches.Add(currentMatch);
        }
    }

    /// <summary>
    /// Assegna un nuovo stile ai due segmenti e aggiunge la coppia alla lista dei match unici.
    /// </summary>
    private static void ApplyNewStyle(
        List<MatchSegment> currentMatch,
        int styleClassCount,
        List<List<MatchSegment>> uniqueMatches)
    {
        currentMatch[0].SetStyleClass(styleClassCount);
        currentMatch[1].SetStyleClass(styleClassCount);
        uniqueMatches.Add(currentMatch);
    }

    /// <summary>
    /// Inizializza il primo match assegnandogli una classe di stile fissa (ad es. 0),
    /// e lo aggiunge ai match unici.
    /// </summary>
    private static void InitializeFirstMatchStyle(
        List<MatchSegment> firstMatch,
        List<List<MatchSegment>> uniqueMatches)
    {
        firstMatch[0].SetStyleClass(0);
        firstMatch[1].SetStyleClass(0);
        uniqueMatches.Add(firstMatch);
    }
}
