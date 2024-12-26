using SimilarityTextComparison.Domain.Models.Matching;

namespace SimilarityTextComparison.Application.Interfaces;

public interface IMatchStep
{
    Task ExecuteAsync(MatchingContext context);
}