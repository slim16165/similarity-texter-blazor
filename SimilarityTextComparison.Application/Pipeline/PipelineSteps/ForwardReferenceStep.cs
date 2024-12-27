using SimilarityTextComparison.Domain.Interfaces.Matching;
using SimilarityTextComparison.Domain.Models.TextPreProcessing;

namespace SimilarityTextComparison.Application.Pipeline.PipelineSteps;

public class ForwardReferenceStep : IPipelineStep
{
    private readonly IForwardReferenceManager _forwardReferenceManager;

    public ForwardReferenceStep(IForwardReferenceManager forwardReferenceManager)
    {
        _forwardReferenceManager = forwardReferenceManager;
    }

    public Task ExecuteAsync(MatchingContext context)
    {
        // Creazione delle forward references per il testo sorgente e target
        if (context.SourceText is { Tokens: not null } && context.TargetText is { Tokens: not null })
        {
            var unifiedTokens = context.SourceText.Tokens.Concat(context.TargetText.Tokens).ToList();
            context.UnifiedForwardReferences = _forwardReferenceManager.CreateForwardReferences(unifiedTokens, [context.SourceText, context.TargetText]);
        }

        return Task.CompletedTask;
    }
}