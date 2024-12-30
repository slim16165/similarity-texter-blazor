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
        // Tokenizza separatamente sorgente e target
        var sourceTokens = _tokenizer.Tokenize(context.SourceText.Text);
        var targetTokens = _tokenizer.Tokenize(context.TargetText.Text);

        // Inserisci i token sorgente nella lista unificata
        context.SourceText.TkBeginPos = context.UnifiedTokens.Count;
        context.UnifiedTokens.AddRange(sourceTokens);
        context.SourceText.TkEndPos = context.UnifiedTokens.Count;

        // Inserisci i token target subito dopo i token sorgente
        context.TargetText.TkBeginPos = context.UnifiedTokens.Count;
        context.UnifiedTokens.AddRange(targetTokens);
        context.TargetText.TkEndPos = context.UnifiedTokens.Count;

        // Se vuoi conservare i token separati a scopo di debug
        context.SourceText.Tokens = sourceTokens;
        context.TargetText.Tokens = targetTokens;

        return Task.CompletedTask;
    }
}