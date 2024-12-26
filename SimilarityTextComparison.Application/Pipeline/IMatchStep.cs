using SimilarityTextComparison.Domain.Models.Matching;

namespace SimilarityTextComparison.Application.Pipeline;

public interface IMatchStep
{
    Task ExecuteAsync(MatchingContext context);
}