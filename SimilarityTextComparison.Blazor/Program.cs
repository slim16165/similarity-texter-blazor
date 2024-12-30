using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SimilarityTextComparison.Application.Infrastructure;
using SimilarityTextComparison.Blazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Registrazione dei servizi di Blazor e Applicativi tramite l'estensione
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSimilarityTextComparisonServices();

// Costruzione del provider e inizializzazione della configurazione è già gestito nell'estensione


// Esegui l'host
await builder.Build().RunAsync();
