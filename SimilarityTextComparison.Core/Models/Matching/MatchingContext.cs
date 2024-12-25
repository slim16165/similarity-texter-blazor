using SimilarityTextComparison.Domain.Models.TextPreProcessing;

namespace SimilarityTextComparison.Domain.Models.Matching;

public class MatchingContext
{
    public ProcessedText SourceText { get; set; }
    public ProcessedText TargetText { get; set; }
    public Dictionary<int, int> ForwardReferences { get; set; }
    public List<Token> Tokens { get; set; }
    public List<List<MatchSegment>> MatchingSegments { get; set; }

    public MatchingContext()
    {
        MatchingSegments = new List<List<MatchSegment>>();
    }
}