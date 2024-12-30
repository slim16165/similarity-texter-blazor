using Microsoft.Extensions.DependencyInjection;
using SimilarityTextComparison.Application.Infrastructure;
using SimilarityTextComparison.Application.Pipeline;
using SimilarityTextComparison.Domain.Models.TextPreProcessing;
using SimilarityTextComparison.Infrastructure.Services;

namespace SimilarityTextComparison.TestConsole;

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

        var config = serviceProvider.GetRequiredService<TextComparisonConfiguration>();
        await config.InitializeAsync();

        Console.WriteLine(config.ToString());

        // Risoluzione dei servizi richiesti
        var matchingPipeline = serviceProvider.GetRequiredService<MatchingPipeline>();

        // Testo di esempio
        //var sourceTextString = "the quick brown fox the quick; jumps over the lazy fox";
        //var targetTextString = "jumps over the lazy dog the quick";
        var sourceTextString = "Testo 1: La volpe marrone salta rapidamente sopra il cane pigro. Il sole splende alto nel cielo azzurro. Camminare tra gli alberi è rilassante e rigenerante. Un vecchio proverbio dice che il mattino ha l'oro in bocca.";
        var targetTextString = "Testo 2: La volpe agile salta velocemente sopra un cane addormentato. Il sole splende alto nel cielo azzurro. Passeggiare tra le foreste è calmante e rinvigorente. Si dice spesso che il mattino porti opportunità d'oro.";

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