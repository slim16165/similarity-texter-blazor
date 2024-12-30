using SimilarityTextComparison.Domain.Interfaces.Matching;
using SimilarityTextComparison.Domain.Models.Matching;

namespace SimilarityTextComparison.Domain.Services.Matching;

/// <summary>
/// Classe responsabile di unire (mergere) segmenti di match che si sovrappongono o sono duplicati,
/// producendo una lista di match unici e ordinati.
/// </summary>
public class MatchedSegmentMerger : IMatchSegmentMerger
{
    /// <inheritdoc />
    public List<List<MatchSegment>> MergeSegments(List<List<MatchSegment>> matches)
    {
        // Ordiniamo in base al secondo segment della coppia (index = 1) per coerenza
        var sortedMatches = SortMatches(matches, 1);

        var uniqueMatches = new List<List<MatchSegment>>();
        foreach (var match in sortedMatches)
        {
            AssignMatchToUniqueMatches(match, uniqueMatches);
        }

        return uniqueMatches;
    }

    /// <summary>
    /// Assegna un singolo match (coppia) alla collezione dei match unici, verificando la sovrapposizione.
    /// </summary>
    private static void AssignMatchToUniqueMatches(
        List<MatchSegment> currentMatch,
        List<List<MatchSegment>> uniqueMatches)
    {
        if (!uniqueMatches.Any())
        {
            InitializeFirstMatch(currentMatch, uniqueMatches);
            return;
        }

        var lastUniqueMatch = uniqueMatches.Last()[1]; // segment target dell'ultimo match
        var current = currentMatch[1];

        // Se non si sovrappone
        if (lastUniqueMatch.IsNonOverlapping(current))
        {
            uniqueMatches.Add(currentMatch);
        }
        // Se c’è estensione
        else if (lastUniqueMatch.CanExtendOverlap(current))
        {
            // Richiama un eventuale merge logico su lastUniqueMatch (o current)
            lastUniqueMatch.ExtendOverlap(current);

            // E infine aggiunge la coppia
            uniqueMatches.Add(currentMatch);
        }
        else
        {
            // Altrimenti, semplicemente aggiungi
            uniqueMatches.Add(currentMatch);
        }
    }

    /// <summary>
    /// Aggiunge il primo match (coppia) nella lista.
    /// </summary>
    private static void InitializeFirstMatch(
        List<MatchSegment> firstMatch,
        List<List<MatchSegment>> uniqueMatches)
    {
        uniqueMatches.Add(firstMatch);
    }

    /// <summary>
    /// Ordina le liste di match in base alla posizione iniziale dei segmenti 
    /// e, in caso di parità, in base alla lunghezza (discendente).
    /// </summary>
    private static List<List<MatchSegment>> SortMatches(
        List<List<MatchSegment>> matches,
        int index)
    {
        var sorted = new List<List<MatchSegment>>(matches);

        // Esempio di ordinamento: 
        // - Prima per TokenBeginPosition crescente
        // - Poi per MatchLength decrescente
        sorted.Sort((a, b) =>
        {
            int comparePos = a[index].TokenBeginPosition.CompareTo(b[index].TokenBeginPosition);
            return comparePos != 0
                ? comparePos
                : b[index].MatchLength.CompareTo(a[index].MatchLength);
        });

        return sorted;
    }
}
