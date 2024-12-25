using SimilarityTextComparison.Domain.Services.Matching;

namespace SimilarityTextComparison.Application.Interfaces
{
    public interface IMatchStep
    {
        Task ExecuteAsync(MatchingContext context);
    }
}