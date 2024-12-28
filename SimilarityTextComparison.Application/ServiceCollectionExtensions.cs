using Blazored.LocalStorage;
using Microsoft.Extensions.DependencyInjection;
using SimilarityTextComparison.Application.Pipeline;
using SimilarityTextComparison.Application.Pipeline.PipelineSteps;
using SimilarityTextComparison.Domain.Interfaces.Matching;
using SimilarityTextComparison.Domain.Interfaces.Styling;
using SimilarityTextComparison.Domain.Interfaces.TextProcessing;
using SimilarityTextComparison.Domain.Services.Matching;
using SimilarityTextComparison.Domain.Services.Styling;
using SimilarityTextComparison.Domain.Services.TextPreProcessing;
using SimilarityTextComparison.Infrastructure.Services;

namespace SimilarityTextComparison.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSimilarityTextComparisonServices(this IServiceCollection services)
    {
        // Registrazione delle implementazioni di dominio
        services.AddScoped<IForwardReferenceManager, ForwardReferenceManager>();
        services.AddScoped<IMatcher, Matcher>();
        services.AddScoped<ITextPreparationService, TextPreparationService>();
        services.AddScoped<ITextInputReader, TextInputReader>();
        services.AddScoped<ITextProcessor, TextProcessor>();
        services.AddScoped<ITokenizer, Tokenizer>();

        // Aggiungi questa riga per registrare IMatchSegmentMerger
        services.AddScoped<IMatchSegmentMerger, MatchedSegmentMerger>();

        // Aggiungi questa riga per registrare IStyleApplier
        services.AddScoped<IStyleApplier, StyleApplier>();

        // Registrazione degli step della pipeline
        services.AddPipelineSteps();

        // Registrazione della pipeline
        services.AddScoped<MatchingPipeline>();

        // Configurazione per Storage e LocalStorage
        services.AddBlazoredLocalStorage();
        services.AddScoped<IStorageService, StorageService>();

        // Registrazione della configurazione
        services.AddScoped<TextComparisonConfiguration>(sp =>
        {
            var storageService = sp.GetRequiredService<IStorageService>();
            var config = new TextComparisonConfiguration(storageService);
            config.InitializeAsync().Wait(); // Sincronizza l'inizializzazione
            return config;
        });

        return services;
    }

    private static IServiceCollection AddPipelineSteps(this IServiceCollection services)
    {
        services.AddScoped<IPipelineStep, TextCleaningStep>();
        services.AddScoped<IPipelineStep, TokenizationStep>();
        services.AddScoped<IPipelineStep, ForwardReferenceStep>();
        services.AddScoped<IPipelineStep, MatcherStep>();
        services.AddScoped<IPipelineStep, MatchSegmentMergerStep>();
        services.AddScoped<IPipelineStep, StyleApplierStep>();

        return services;
    }
}