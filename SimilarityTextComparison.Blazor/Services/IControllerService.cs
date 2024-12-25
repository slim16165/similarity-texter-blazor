using SimilarityTextComparison.Domain.Models.Matching;

namespace SimilarityTextComparison.Blazor.Services;

public interface IControllerService
{
    Task<List<List<MatchSegment>>> CompareTextsAsync(string input1, string input2);
}