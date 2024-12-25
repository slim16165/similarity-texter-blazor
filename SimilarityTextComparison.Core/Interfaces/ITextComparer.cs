using SimilarityTextComparison.Core.Models.Comparison;
using SimilarityTextComparison.Core.Models.TextProcessing;

namespace SimilarityTextComparison.Core.Interfaces;

public interface ITextComparer
{
    Task<List<List<MatchSegment>>> CompareAsync(List<InputInfo> inputTexts);
}