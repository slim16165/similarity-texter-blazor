using SimilarityTextComparison.Domain.Models.Comparison;
using SimilarityTextComparison.Domain.Models.TextProcessing;

namespace SimilarityTextComparison.Application.Interfaces;

public interface ITextComparer
{
    Task<List<List<MatchSegment>>> CompareAsync(List<InputInfo> inputTexts);
}