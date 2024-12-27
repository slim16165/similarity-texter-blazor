using SimilarityTextComparison.Domain.Interfaces.TextProcessing;

namespace SimilarityTextComparison.Application.Pipeline.PipelineSteps;

public class TokenizationStep : IPipelineStep
{
    private readonly ITokenizer _tokenizer;

    public TokenizationStep(ITokenizer tokenizer)
    {
        _tokenizer = tokenizer;
    }

    public Task ExecuteAsync(MatchingContext context)
    {
        context.SourceText.Tokens = _tokenizer.Tokenize(context.SourceText.Text);
        context.TargetText.Tokens = _tokenizer.Tokenize(context.TargetText.Text);
        return Task.CompletedTask;
    }
}