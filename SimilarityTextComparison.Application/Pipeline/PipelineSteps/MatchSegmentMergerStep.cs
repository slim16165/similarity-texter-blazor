using SimilarityTextComparison.Domain.Interfaces.Matching;

namespace SimilarityTextComparison.Application.Pipeline.PipelineSteps;

public class MatchSegmentMergerStep : IMatchStep
{
    private readonly IMatchSegmentMerger _segmentMerger;

    public MatchSegmentMergerStep(IMatchSegmentMerger segmentMerger)
    {
        _segmentMerger = segmentMerger;
    }

    public Task ExecuteAsync(MatchingContext context)
    {
        context.MatchingSegments = _segmentMerger.MergeSegments(context.MatchingSegments);
        return Task.CompletedTask;
    }
}