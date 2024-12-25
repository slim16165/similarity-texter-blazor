namespace SimilarityTextComparison.Core.Services.Matching
{
    public interface IMatchStep
    {
        Task ExecuteAsync(MatchingContext context);
    }
}