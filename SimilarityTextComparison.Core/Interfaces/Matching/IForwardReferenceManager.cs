using SimilarityTextComparison.Domain.Models.Matching;
using SimilarityTextComparison.Domain.Models.TextPreProcessing;

namespace SimilarityTextComparison.Domain.Interfaces.Matching;

public interface IForwardReferenceManager
{
    List<ForwardReference> CreateForwardReferences(List<Token> allTokens, List<ProcessedText> texts);
}