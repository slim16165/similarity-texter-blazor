using SimilarityTextComparison.Domain.Models.Matching;
using SimilarityTextComparison.Domain.Models.TextPreProcessing;

namespace SimilarityTextComparison.Application.Pipeline;

public class MatchingContext
{
    public ProcessedText SourceText { get; set; }
    public ProcessedText TargetText { get; set; }
    public Dictionary<int, int> ForwardReferences { get; set; }
    public List<List<MatchSegment>> MatchingSegments { get; set; }

    public MatchingContext()
    {
        ForwardReferences = new Dictionary<int, int>();
        MatchingSegments = new List<List<MatchSegment>>();
    }
}