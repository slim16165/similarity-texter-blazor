using SimilarityTextComparison.Domain.Models.Comparison;

namespace SimilarityTextComparison.Blazor.Services;

public interface IControllerService
{
    Task<List<List<MatchSegment>>> CompareTextsAsync(string input1, string input2);
}