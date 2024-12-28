using Microsoft.Extensions.DependencyInjection;
using SimilarityTextComparison.Application.Pipeline;
using SimilarityTextComparison.Domain.Interfaces.Matching;
using SimilarityTextComparison.Domain.Interfaces.TextProcessing;
using SimilarityTextComparison.Domain.Models.TextPreProcessing;
using SimilarityTextComparison.Domain.Models.Matching;
using System.Linq;
using System.Collections.Generic;
using SimilarityTextComparison.Application;
using SimilarityTextComparison.Infrastructure.Services;

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
            SourceText = new ProcessedText(sourceTextString, new ProcessedText.TextStatistics(sourceTextString), new List<Token>()),
            TargetText = new ProcessedText(targetTextString, new ProcessedText.TextStatistics(targetTextString), new List<Token>())
        };

        // Esecuzione della pipeline di matching
        await matchingPipeline.ExecuteAsync(context);

        var matches = context.MatchingSegments;

        // Output dei risultati
        foreach (var matchPair in matches)
        {
            var sourceMatch = matchPair[0];
            var targetMatch = matchPair[1];

            // Recupero delle parole corrispondenti dal testo sorgente
            var sourceMatchedWords = context.SourceText.Tokens
                .Skip(sourceMatch.BeginPosition)
                .Take(sourceMatch.Length)
                .Select(t => t.Text);

            // Recupero delle parole corrispondenti dal testo target
            var targetMatchedWords = context.TargetText.Tokens
                .Skip(targetMatch.BeginPosition - context.SourceText.Tokens.Count)
                .Take(targetMatch.Length)
                .Select(t => t.Text);

            Console.WriteLine($"Source Match: [{sourceMatch.BeginPosition}, {sourceMatch.Length}] -> '{string.Join(" ", sourceMatchedWords)}'");
            Console.WriteLine($"Target Match: [{targetMatch.BeginPosition}, {targetMatch.Length}] -> '{string.Join(" ", targetMatchedWords)}'");
            Console.WriteLine();
        }

        if (!matches.Any())
        {
            Console.WriteLine("No similarities found.");
        }
    }
}