using SimilarityTextComparison.Application.Interfaces;
using SimilarityTextComparison.Domain.Interfaces.Matching;
using SimilarityTextComparison.Domain.Models.Matching;

namespace SimilarityTextComparison.Application.Services.Matching;

public class MatcherStep : IMatchStep
{
    private readonly IMatcher _matcher;

    public MatcherStep(IMatcher matcher)
    {
        _matcher = matcher;
    }

    public Task ExecuteAsync(MatchingContext context)
    {
        var matches = _matcher.FindMatches(
            sourceTextIndex: 0,
            targetTextIndex: 1,
            sourceText: context.SourceText,
            targetText: context.TargetText,
            forwardReferences: context.ForwardReferences,
            tokens: context.Tokens);

        context.MatchingSegments.AddRange(matches);
        return Task.CompletedTask;
    }
}