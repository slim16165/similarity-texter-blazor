using SimilarityTextComparison.Domain.Interfaces.Matching;

namespace SimilarityTextComparison.Application.Pipeline.PipelineSteps;

public class MatcherStep : IPipelineStep
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
            unifiedForwardReferences: context.UnifiedForwardReferences,
            unifiedTokens: context.SourceText.Tokens.Concat(context.TargetText.Tokens).ToList());

        context.MatchingSegments.AddRange(matches);
        return Task.CompletedTask;
    }
}