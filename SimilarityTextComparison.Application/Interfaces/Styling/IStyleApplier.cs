using SimilarityTextComparison.Domain.Models.Matching;

namespace SimilarityTextComparison.Application.Interfaces.Styling;

public interface IStyleApplier
{
    List<List<MatchSegment>> ApplyStyles(List<List<MatchSegment>> matches);
}