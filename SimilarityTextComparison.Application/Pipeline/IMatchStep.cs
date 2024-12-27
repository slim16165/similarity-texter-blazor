namespace SimilarityTextComparison.Application.Pipeline;

public interface IMatchStep
{
    Task ExecuteAsync(MatchingContext context);
}