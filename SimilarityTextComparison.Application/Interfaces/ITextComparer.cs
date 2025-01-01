using SimilarityTextComparison.Domain.Models.Matching;
using SimilarityTextComparison.Domain.Models.TextPreProcessing;

namespace SimilarityTextComparison.Application.Interfaces;

public interface ITextComparer
{
    Task<List<List<MatchSegment>>> CompareAsync(List<InputInfo> inputTexts);
    Task<List<List<MatchSegment>>> CompareAsync(string text1, string text2);
}