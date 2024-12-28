using Microsoft.Extensions.DependencyInjection;
using SimilarityTextComparison.Application.Pipeline;
using SimilarityTextComparison.Domain.Interfaces.Matching;
using SimilarityTextComparison.Domain.Interfaces.TextProcessing;
using SimilarityTextComparison.Domain.Models.TextPreProcessing;
using SimilarityTextComparison.Domain.Models.Matching;
using System.Linq;
using System.Collections.Generic;
using SimilarityTextComparison.Infrastructure.Services;
using SimilarityTextComparison.Application.Infrastructure;

class Program
{
    static async Task Main(string[] args)
    {
        // Configurazione dei servizi
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSimilarityTextComparisonServices();
        serviceCollection.AddScoped<IStorageService, FileStorageService>();

        // Costruzione del service provider
        var serviceProvider = serviceCollection.BuildServiceProvider();


        // Risoluzione dei servizi richiesti
        var matchingPipeline = serviceProvider.GetRequiredService<MatchingPipeline>();

        // Testo di esempio
        var sourceTextString = "the quick brown fox the quick";
        var targetTextString = "jumps over the lazy dog the quick";

        // Creazione del MatchingContext con testi non preprocessati
        var context = new MatchingContext
        {
            SourceText = new ProcessedText(
                sourceTextString,
                new ProcessedText.TextStatistics(sourceTextString),
                new List<Token>()
            ),
            TargetText = new ProcessedText(
                targetTextString,
                new ProcessedText.TextStatistics(targetTextString),
                new List<Token>()
            )
        };

        // Esegui la pipeline
        await matchingPipeline.ExecuteAsync(context);

        // Stampa i risultati
        var matches = context.MatchingSegments;
        if (!matches.Any())
        {
            Console.WriteLine("No similarities found.");
            return;
        }

        // Usa la lista unificata
        var unifiedTokens = context.UnifiedTokens;

        foreach (var matchPair in matches)
        {
            var src = matchPair[0];
            var trg = matchPair[1];

            // Recupero delle parole corrispondenti dal testo unificato
            var sourceWords = unifiedTokens
                .Skip(src.BeginPosition)
                .Take(src.Length)
                .Select(t => t.Text);

            var targetWords = unifiedTokens
                .Skip(trg.BeginPosition)
                .Take(trg.Length)
                .Select(t => t.Text);

            Console.WriteLine(
                $"Source Match: [{src.BeginPosition}, {src.Length}] -> '{string.Join(" ", sourceWords)}'");
            Console.WriteLine(
                $"Target Match: [{trg.BeginPosition}, {trg.Length}] -> '{string.Join(" ", targetWords)}'");
            Console.WriteLine();
        }
    }
}