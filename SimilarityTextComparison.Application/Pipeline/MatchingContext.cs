using SimilarityTextComparison.Domain.Models.Matching;
using SimilarityTextComparison.Domain.Models.TextPreProcessing;

namespace SimilarityTextComparison.Application.Pipeline;

public class MatchingContext
{
    public ProcessedText SourceText { get; set; }
    public ProcessedText TargetText { get; set; }

    // Risultato principale: lista di match segment
    public List<List<MatchSegment>> MatchingSegments { get; set; }

    // Forward references unificate
    public List<ForwardReference> UnifiedForwardReferences { get; set; }

    // Lista unificata di token (sorgente + target)
    public List<Token> UnifiedTokens { get; set; }

    public MatchingContext()
    {
        MatchingSegments = new List<List<MatchSegment>>();
        UnifiedForwardReferences = new List<ForwardReference>();
        UnifiedTokens = new List<Token>();
    }
}