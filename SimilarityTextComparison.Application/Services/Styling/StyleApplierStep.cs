using SimilarityTextComparison.Application.Interfaces;
using SimilarityTextComparison.Application.Interfaces.Styling;
using SimilarityTextComparison.Domain.Models.Matching;

namespace SimilarityTextComparison.Application.Services.Styling;

public class StyleApplierStep : IMatchStep
{
    private readonly IStyleApplier _styleApplier;

    public StyleApplierStep(IStyleApplier styleApplier)
    {
        _styleApplier = styleApplier;
    }

    public Task ExecuteAsync(MatchingContext context)
    {
        context.MatchingSegments = _styleApplier.ApplyStyles(context.MatchingSegments);
        return Task.CompletedTask;
    }
}