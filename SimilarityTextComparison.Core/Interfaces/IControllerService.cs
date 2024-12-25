using SimilarityTextComparison.Core.Models.Comparison;

namespace SimilarityTextComparison.Core.Interfaces;

public interface IControllerService
{
    Task<List<List<MatchSegment>>> CompareTextsAsync(string input1, string input2);
}