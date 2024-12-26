using SimilarityTextComparison.Domain.Models.Matching;

namespace SimilarityTextComparison.Domain.Interfaces.Matching;

public interface IMatchSegmentMerger
{
    List<List<MatchSegment>> MergeSegments(List<List<MatchSegment>> matches);
}