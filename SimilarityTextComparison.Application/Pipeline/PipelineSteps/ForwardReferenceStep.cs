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
        // Usa la lista unificata e i relativi TkBeginPos / TkEndPos
        if (context.UnifiedTokens is { Count: > 0 })
        {
            var texts = new List<ProcessedText> { context.SourceText, context.TargetText };
            context.UnifiedForwardReferences = _forwardReferenceManager.CreateForwardReferences(
                context.UnifiedTokens,
                texts
            );
        }

        return Task.CompletedTask;
    }
}