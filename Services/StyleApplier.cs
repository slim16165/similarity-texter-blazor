using ChatGPT_Splitter_Blazor_New.TextComparer.Model.Comparison;

namespace ChatGPT_Splitter_Blazor_New.TextComparer.Services;

public class StyleApplier
{
    public int UniqueMatches { get; private set; }

    public List<List<MatchSegment>> ApplyStyles(List<List<MatchSegment>> matches)
    {
        var sortedMatches = SortMatches(matches, 1);
        int styleClassCount = 1;
        var uniqueMatches = new List<List<MatchSegment>>();

        foreach (var match in sortedMatches)
        {
            AssignStyleToMatch(match, uniqueMatches, ref styleClassCount);
        }

        UniqueMatches = uniqueMatches.Count;
        return uniqueMatches;
    }

    private static void AssignStyleToMatch(List<MatchSegment> currentMatch, List<List<MatchSegment>> uniqueMatches, ref int styleClassCount)
    {
        if (!uniqueMatches.Any())
        {
            InitializeFirstMatchStyle(currentMatch, uniqueMatches);
            return;
        }

        var lastUniqueMatch = uniqueMatches.Last()[1];
        var current = currentMatch[1];

        if (IsNonOverlapping(lastUniqueMatch, current))
        {
            currentMatch[0].SetStyleClass(styleClassCount);
            currentMatch[1].SetStyleClass(styleClassCount);
            uniqueMatches.Add(currentMatch);
            styleClassCount++;
        }
        else if (CanExtendOverlap(lastUniqueMatch, current))
        {
            ExtendOverlapStyles(lastUniqueMatch, current);
            uniqueMatches.Add(currentMatch);
        }
    }

    private static void InitializeFirstMatchStyle(List<MatchSegment> firstMatch, List<List<MatchSegment>> uniqueMatches)
    {
        firstMatch[0].SetStyleClass(0);
        firstMatch[1].SetStyleClass(0);
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

    private static void ExtendOverlapStyles(MatchSegment lastUniqueMatch, MatchSegment currentMatch)
    {
        if (lastUniqueMatch.Unit != currentMatch.Unit)
        {
            throw new InvalidOperationException("Le unità di posizione di lastUniqueMatch e currentMatch non sono coerenti.");
        }

        // Verifica se la classe di stile già contiene "overlapping"
        var styleClass = lastUniqueMatch.StyleClass.EndsWith(" overlapping")
            ? lastUniqueMatch.StyleClass
            : $"{lastUniqueMatch.StyleClass} overlapping";

        // Applica la nuova classe di stile a entrambi i MatchSegment
        lastUniqueMatch.SetStyleClass(styleClass);
        currentMatch.SetStyleClass(styleClass);

        // Calcola il nuovo MatchLength basato sulle posizioni begin
        currentMatch.MatchLength = currentMatch.BeginPosition - lastUniqueMatch.BeginPosition;
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