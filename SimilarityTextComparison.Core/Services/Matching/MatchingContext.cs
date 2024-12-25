using SimilarityTextComparison.Domain.Models.Comparison;
using SimilarityTextComparison.Domain.Models.TextProcessing;

namespace SimilarityTextComparison.Domain.Services.Matching;

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