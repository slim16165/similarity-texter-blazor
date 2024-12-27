namespace SimilarityTextComparison.Application.Pipeline;

public interface IPipelineStep
{
    Task ExecuteAsync(MatchingContext context);
}