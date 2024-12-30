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

        // Log dei token
        Console.WriteLine($"Source Tokens ({sourceTokens.Count}): {string.Join(", ", sourceTokens.Select(t => t.Text))}");
        Console.WriteLine($"Target Tokens ({targetTokens.Count}): {string.Join(", ", targetTokens.Select(t => t.Text))}");


        // Inserisci i token sorgente nella lista unificata
        context.SourceText.TkBeginPos = context.UnifiedTokens.Count;
        context.UnifiedTokens.AddRange(sourceTokens);
        context.SourceText.TkEndPos = context.UnifiedTokens.Count;

        // Inserisci i token target subito dopo i token sorgente
        context.TargetText.TkBeginPos = context.UnifiedTokens.Count;
        context.UnifiedTokens.AddRange(targetTokens);
        context.TargetText.TkEndPos = context.UnifiedTokens.Count;

        // Log delle posizioni
        Console.WriteLine($"SourceText: TkBeginPos={context.SourceText.TkBeginPos}, TkEndPos={context.SourceText.TkEndPos}");
        Console.WriteLine($"TargetText: TkBeginPos={context.TargetText.TkBeginPos}, TkEndPos={context.TargetText.TkEndPos}");


        // Se vuoi conservare i token separati a scopo di debug
        context.SourceText.Tokens = sourceTokens;
        context.TargetText.Tokens = targetTokens;

        MatchingContext.UnifiedTokensStatic = sourceTokens.Concat(targetTokens).ToList();

        return Task.CompletedTask;
    }
}