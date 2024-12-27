using SimilarityTextComparison.Domain.Models.Matching;
using SimilarityTextComparison.Domain.Models.TextPreProcessing;

namespace SimilarityTextComparison.Application.Pipeline;

public class MatchingContext
{
    public ProcessedText SourceText { get; set; }
    public ProcessedText TargetText { get; set; }
    public List<List<MatchSegment>> MatchingSegments { get; set; }
    public List<ForwardReference> UnifiedForwardReferences { get; set; }

    public MatchingContext()
    {
        UnifiedForwardReferences = new List<ForwardReference>() ;
        MatchingSegments = new List<List<MatchSegment>>();
    }
}