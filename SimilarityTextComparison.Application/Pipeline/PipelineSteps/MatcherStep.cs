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
            sourceTextIndex: 0,       // Indici logici: 0 e 1
            targetTextIndex: 1,
            sourceText: context.SourceText,
            targetText: context.TargetText,
            unifiedForwardReferences: context.UnifiedForwardReferences,
            unifiedTokens: context.UnifiedTokens // ecco la singola lista
        );

        context.MatchingSegments.AddRange(matches);
        return Task.CompletedTask;
    }
}