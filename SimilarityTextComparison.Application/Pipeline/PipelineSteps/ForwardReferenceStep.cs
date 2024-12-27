using SimilarityTextComparison.Domain.Interfaces.Matching;

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
        context.ForwardReferences = _forwardReferenceManager.CreateForwardReferences(context.SourceText);
        return Task.CompletedTask;
    }
}