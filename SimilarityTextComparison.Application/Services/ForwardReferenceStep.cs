using SimilarityTextComparison.Application.Interfaces;
using SimilarityTextComparison.Domain.Interfaces;
using SimilarityTextComparison.Domain.Services.Matching;

namespace SimilarityTextComparison.Application.Services
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
