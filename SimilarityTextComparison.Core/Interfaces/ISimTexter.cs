using SimilarityTextComparison.Core.Models.Comparison;
using SimilarityTextComparison.Core.Models.TextProcessing;

namespace SimilarityTextComparison.Core.Interfaces;

public interface ISimTexter
{
    Task<List<List<MatchSegment>>> CompareAsync(List<MyInputText> inputTexts);
}