using Moq;
using SimilarityTextComparison.Domain.Services.Matching;
using SimilarityTextComparison.Infrastructure.Services;
using SimilarityTextComparison.Domain.Models.TextPreProcessing;
using SimilarityTextComparison.Domain.Models.Matching;
using SimilarityTextComparison.Domain.Models.Position.Enum;
using Xunit;
using Xunit.Abstractions;
using System.Collections.Generic;
using SimilarityTextComparison.Domain.Models.Position;

namespace SimilarityTextComparison.TestProject
{
    /// <summary>
    /// Classe base per i test che fornisce configurazione comune, mock e metodi helper.
    /// </summary>
    public abstract class BaseTest
    {
        protected readonly Mock<IStorageService> MockStorage;
        protected readonly TextComparisonConfiguration Config;
        protected readonly ForwardReferenceManager ForwardReferenceManager;
        protected readonly Matcher Matcher;
        protected readonly ITestOutputHelper Output;

        protected BaseTest(ITestOutputHelper output)
        {
            Output = output;

            // Setup del mock per IStorageService
            MockStorage = new Mock<IStorageService>();
            MockStorage.Setup(s => s.GetItemAsync<bool>("ignoreLetterCase")).ReturnsAsync(true);
            MockStorage.Setup(s => s.GetItemAsync<bool>("ignoreNumbers")).ReturnsAsync(true);
            MockStorage.Setup(s => s.GetItemAsync<bool>("ignorePunctuation")).ReturnsAsync(true);
            MockStorage.Setup(s => s.GetItemAsync<bool>("replaceUmlaut")).ReturnsAsync(true);
            MockStorage.Setup(s => s.GetItemAsync<int>("minMatchLength")).ReturnsAsync(2);
            MockStorage.Setup(s => s.GetItemAsync<bool>("isHtmlInput")).ReturnsAsync(false);

            // Inizializzazione della configurazione
            Config = new TextComparisonConfiguration(MockStorage.Object);
            Config.InitializeAsync().Wait();

            // Inizializzazione dei servizi dipendenti
            ForwardReferenceManager = new ForwardReferenceManager(Config);
            Matcher = new Matcher(Config);
        }

        /// <summary>
        /// Crea una lista di token a partire da una lista di parole.
        /// </summary>
        /// <param name="words">Le parole da convertire in token.</param>
        /// <returns>Lista di token.</returns>
        protected List<Token> CreateTokens(List<string> words)
        {
            var tokens = new List<Token>();
            int pos = 0;

            foreach (var word in words)
            {
                if (string.IsNullOrWhiteSpace(word))
                    continue;

                var begin = pos;
                var end = pos + word.Length;
                tokens.Add(new Token(word, begin, end));
                pos = end + 1; // +1 per lo spazio
            }

            return tokens;
        }

        /// <summary>
        /// Crea un'istanza di ProcessedText.
        /// </summary>
        /// <param name="text">Il testo processato.</param>
        /// <param name="allTokens">Tutti i token disponibili.</param>
        /// <param name="startIndex">Indice di inizio dei token per questo testo.</param>
        /// <param name="count">Numero di token per questo testo.</param>
        /// <returns>Istanza di ProcessedText.</returns>
        protected ProcessedText CreateProcessedText(string text, List<Token> allTokens, int startIndex, int count)
        {
            return new ProcessedText(
                text,
                new ProcessedText.TextStatistics(text),
                allTokens.GetRange(startIndex, count)
            )
            {
                TkBeginPos = startIndex,
                TkEndPos = startIndex + count
            };
        }
    }

    

    
}
