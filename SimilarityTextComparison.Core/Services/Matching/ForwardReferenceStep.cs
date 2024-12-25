using SimilarityTextComparison.Core.Services.Comparison;

namespace SimilarityTextComparison.Core.Services.Matching
{
    public class ForwardReferenceStep : IMatchStep
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
}
