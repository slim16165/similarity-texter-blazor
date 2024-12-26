using SimilarityTextComparison.Domain.Interfaces.Matching;
using SimilarityTextComparison.Domain.Models.Matching;

namespace SimilarityTextComparison.Domain.Services.Matching;

public class MatchedSegmentMerger : IMatchSegmentMerger
{
    public List<List<MatchSegment>> MergeSegments(List<List<MatchSegment>> matches)
    {
        var sortedMatches = SortMatches(matches, 1);
        var uniqueMatches = new List<List<MatchSegment>>();

        foreach (var match in sortedMatches)
        {
            AssignMatchToUniqueMatches(match, uniqueMatches);
        }

        return uniqueMatches;
    }

    private static void AssignMatchToUniqueMatches(List<MatchSegment> currentMatch, List<List<MatchSegment>> uniqueMatches)
    {
        if (!uniqueMatches.Any())
        {
            InitializeFirstMatch(currentMatch, uniqueMatches);
            return;
        }

        var lastUniqueMatch = uniqueMatches.Last()[1];
        var current = currentMatch[1];

        if (IsNonOverlapping(lastUniqueMatch, current))
        {
            uniqueMatches.Add(currentMatch);
        }
        else if (CanExtendOverlap(lastUniqueMatch, current))
        {
            ExtendOverlap(lastUniqueMatch, current);
            uniqueMatches.Add(currentMatch);
        }
    }

    private static void InitializeFirstMatch(List<MatchSegment> firstMatch, List<List<MatchSegment>> uniqueMatches)
    {
        uniqueMatches.Add(firstMatch);
    }

    private static bool IsNonOverlapping(MatchSegment lastUniqueMatch, MatchSegment currentMatch)
    {
        return lastUniqueMatch.TokenBeginPosition != currentMatch.TokenBeginPosition &&
               lastUniqueMatch.GetTkEndPosition() - 1 < currentMatch.TokenBeginPosition;
    }

    private static bool CanExtendOverlap(MatchSegment lastUniqueMatch, MatchSegment currentMatch)
    {
        return lastUniqueMatch.GetTkEndPosition() < currentMatch.GetTkEndPosition();
    }

    private static void ExtendOverlap(MatchSegment lastUniqueMatch, MatchSegment currentMatch)
    {
        if (lastUniqueMatch.Unit != currentMatch.Unit)
        {
            throw new InvalidOperationException("Le unità di posizione di lastUniqueMatch e currentMatch non sono coerenti.");
        }

        // Logica di fusione (esempio: aggiornamento di proprietà)
        // Puoi implementare ulteriori logiche di fusione qui se necessario.
    }

    private static List<List<MatchSegment>> SortMatches(List<List<MatchSegment>> matches, int index)
    {
        var sorted = new List<List<MatchSegment>>(matches);
        sorted.Sort((a, b) =>
        {
            int comparePos = a[index].TokenBeginPosition.CompareTo(b[index].TokenBeginPosition);
            return comparePos != 0 ? comparePos : b[index].MatchLength.CompareTo(a[index].MatchLength);
        });
        return sorted;
    }
}