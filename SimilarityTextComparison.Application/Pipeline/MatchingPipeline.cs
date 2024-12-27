using SimilarityTextComparison.Domain.Models.Matching;

namespace SimilarityTextComparison.Application.Pipeline;

public class MatchingPipeline
{
    private readonly IEnumerable<IPipelineStep> _steps;

    public MatchingPipeline(IEnumerable<IPipelineStep> steps)
    {
        _steps = steps;
    }

    public async Task<List<List<MatchSegment>>> ExecuteAsync(MatchingContext context)
    {
        foreach (var step in _steps)
        {
            await step.ExecuteAsync(context);
        }

        return context.MatchingSegments;
    }
}