using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SimilarityTextComparison.Application;
using SimilarityTextComparison.Application.Interfaces;
using SimilarityTextComparison.Application.Pipeline;
using SimilarityTextComparison.Application.Pipeline.PipelineSteps;
using SimilarityTextComparison.Blazor;
using SimilarityTextComparison.Blazor.Services;
using SimilarityTextComparison.Domain.Interfaces.Matching;
using SimilarityTextComparison.Domain.Interfaces.Styling;
using SimilarityTextComparison.Domain.Interfaces.TextProcessing;
using SimilarityTextComparison.Domain.Services.Matching;
using SimilarityTextComparison.Domain.Services.Styling;
using SimilarityTextComparison.Domain.Services.TextPreProcessing;
using SimilarityTextComparison.Infrastructure.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Registrazione dei servizi di Blazor
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Registrazione dei servizi applicativi
builder.Services.AddScoped<ITextComparer, TextComparer>();
builder.Services.AddScoped<MatchingPipeline>();

// Registrazione dei match steps
builder.Services.AddScoped<IMatchStep, ForwardReferenceStep>();
builder.Services.AddScoped<IMatchStep, MatcherStep>();
builder.Services.AddScoped<IMatchStep, MatchSegmentMergerStep>();
builder.Services.AddScoped<IMatchStep, StyleApplierStep>();

// Registrazione delle implementazioni delle interfacce di styling
builder.Services.AddScoped<IMatchSegmentMerger, MatchedSegmentMerger>();
builder.Services.AddScoped<IStyleApplier, StyleApplier>();

// Registrazione dei servizi di dominio
builder.Services.AddScoped<IForwardReferenceManager, ForwardReferenceManager>();
builder.Services.AddScoped<IMatcher, Matcher>();
builder.Services.AddScoped<ITextPreparationService, TextPreparationService>();
builder.Services.AddScoped<ITextInputReader, TextInputReader>();
builder.Services.AddScoped<ITextProcessor, TextProcessor>();
builder.Services.AddScoped<ITokenizer, Tokenizer>();

// Registrazione dei servizi di infrastruttura
builder.Services.AddScoped<IStorageService, StorageService>();
builder.Services.AddScoped<IControllerService, TextComparisonService>();

// Registrazione della configurazione
builder.Services.AddScoped<TextComparisonConfiguration>();


// Aggiungi Blazored LocalStorage
builder.Services.AddBlazoredLocalStorage();

// Costruzione del provider e inizializzazione della configurazione
var host = builder.Build();
var config = host.Services.GetRequiredService<TextComparisonConfiguration>();
await config.InitializeAsync();

// Esegui l'host
await host.RunAsync();
