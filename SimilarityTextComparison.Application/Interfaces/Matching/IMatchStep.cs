using SimilarityTextComparison.Domain.Models.Matching;

namespace SimilarityTextComparison.Application.Interfaces.Matching;

public interface IMatchStep
{
    Task ExecuteAsync(MatchingContext context);
}