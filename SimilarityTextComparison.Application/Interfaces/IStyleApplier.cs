using SimilarityTextComparison.Domain.Models.Comparison;

namespace SimilarityTextComparison.Application.Interfaces;

public interface IStyleApplier
{
    List<List<MatchSegment>> ApplyStyles(List<List<MatchSegment>> matches);
}