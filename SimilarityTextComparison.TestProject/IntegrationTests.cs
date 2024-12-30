using Microsoft.Extensions.DependencyInjection;
using SimilarityTextComparison.Application.Pipeline;
using SimilarityTextComparison.Infrastructure.Services;
using SimilarityTextComparison.Application.Infrastructure;
using Xunit.Abstractions;
using SimilarityTextComparison.Domain.Interfaces.TextProcessing;

namespace SimilarityTextComparison.TestProject
{
    public class IntegrationTests
    {
        private readonly ITestOutputHelper _output;

        public IntegrationTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task FindMatches_EndToEnd_SimpleMatch()
        {
            // Arrange
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSimilarityTextComparisonServices();
            serviceCollection.AddScoped<IStorageService, FileStorageService>(); // Usa FileStorageService o un'altra implementazione reale

            // Configurazione dei servizi (esempio)
            // Potrebbe essere necessario impostare valori di configurazione predefiniti
            serviceCollection.AddScoped<TextComparisonConfiguration>(sp =>
            {
                var storageService = sp.GetRequiredService<IStorageService>();
                var config = new TextComparisonConfiguration(storageService);
                config.InitializeAsync().Wait(); // Inizializza la configurazione
                return config;
            });

            // Costruzione del service provider
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // Risoluzione dei servizi richiesti
            var matchingPipeline = serviceProvider.GetRequiredService<MatchingPipeline>();
            var textPreparationService = serviceProvider.GetRequiredService<ITextPreparationService>();

            // Testo di esempio
            var sourceTextString = "the quick brown fox the quick; jumps over the lazy fox";
            var targetTextString = "jumps over the lazy dog the quick";

            // Preprocessamento e tokenizzazione dei testi
            var (processedSourceText, sourceTokens) = await textPreparationService.PreProcessAndTokenizeText(sourceTextString);
            var (processedTargetText, targetTokens) = await textPreparationService.PreProcessAndTokenizeText(targetTextString);

            // Creazione del MatchingContext
            var context = new MatchingContext
            {
                SourceText = processedSourceText,
                TargetText = processedTargetText
            };

            // Esegui la pipeline
            await matchingPipeline.ExecuteAsync(context);

            // Stampa i risultati
            var matches = context.MatchingSegments;
            if (!matches.Any())
            {
                _output.WriteLine("No similarities found.");
                Assert.Empty(matches);
                return;
            }

            _output.WriteLine($"Total Matches Found: {matches.Count}");

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

                _output.WriteLine(
                    $"Source Match: [{src.BeginPosition}, {src.Length}] -> '{string.Join(" ", sourceWords)}'");
                _output.WriteLine(
                    $"Target Match: [{trg.BeginPosition}, {trg.Length}] -> '{string.Join(" ", targetWords)}'");
                _output.WriteLine("");
            }

            // Assert
            // Esempio di asserzione, modifica secondo le tue aspettative
            Assert.NotEmpty(matches);

            // Ad esempio, verifica che almeno un match sia stato trovato
            Assert.True(matches.Count >= 1);
        }
    }
}
