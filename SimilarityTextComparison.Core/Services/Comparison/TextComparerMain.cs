// TextComparer.cs

using SimilarityTextComparison.Core.Interfaces;
using SimilarityTextComparison.Core.Models.Comparison;
using SimilarityTextComparison.Core.Models.TextProcessing;
using SimilarityTextComparison.Core.Services.Matching;
using SimilarityTextComparison.Core.Services.TextProcessing;

namespace SimilarityTextComparison.Core.Services.Comparison
{
    public class TextComparer : ITextComparer
    {
        private readonly ITextPreparationService _textPreparationService;
        private readonly MatchingPipeline _matchingPipeline;

        public TextComparer(
            ITextPreparationService textPreparationService,
            MatchingPipeline matchingPipeline)
        {
            _textPreparationService = textPreparationService ?? throw new ArgumentNullException(nameof(textPreparationService));
            _matchingPipeline = matchingPipeline ?? throw new ArgumentNullException(nameof(matchingPipeline));
        }


        /// <summary>
        /// Confronta una lista di testi di input e restituisce i segmenti corrispondenti trovati.
        /// </summary>
        /// <param name="inputTexts">Lista dei testi di input da confrontare.</param>
        /// <returns>Lista dei segmenti corrispondenti tra i testi.</returns>
        /// <exception cref="ArgumentException">Se il numero di testi di input è inferiore a 2.</exception>
        public async Task<List<List<MatchSegment>>> CompareAsync(List<InputInfo> inputTexts)
        {
            // Verifica che ci siano almeno due testi da confrontare
            if (inputTexts == null || inputTexts.Count < 2)
                throw new ArgumentException("Sono necessari almeno due testi per il confronto.", nameof(inputTexts));

            // Preprocessa i testi di input: pulizia e tokenizzazione
            var processedTexts = new List<ProcessedText>();
            var allTokens = new List<Token>();
            int currentTokenPosition = 0;

            foreach (var inputText in inputTexts)
            {
                var textStatistics = new TextStatistics(inputText.Text);

                // Pulisce il testo
                var cleanedText = await _textPreparationService.PrepareTextAsync(inputText.Text);

                // Tokenizza il testo pulito
                var tokens = await _textPreparationService.PrepareTextAsync(inputText.Text);

                // Crea un oggetto ProcessedText che rappresenta il testo processato
                var tokenizationInfo = new TokenizationInfo(currentTokenPosition, tokens.Count);
                var processedText = new ProcessedText(inputText.Text, inputText, textStatistics, tokenizationInfo);

                processedTexts.Add(processedText);
                allTokens.AddRange(tokens);
                currentTokenPosition += tokens.Count;
            }

            // Crea il contesto della pipeline
            var context = new MatchingContext
            {
                SourceText = processedTexts[0],
                TargetText = processedTexts[1],
                Tokens = allTokens
            };

            // Esegue la pipeline di matching
            var matchingSegments = await _matchingPipeline.ExecuteAsync(context);

            // Verifica se sono stati trovati segmenti corrispondenti
            if (matchingSegments.Count > 0)
            {
                return matchingSegments;
            }
            else
            {
                throw new InvalidOperationException("Nessuna similarità trovata.");
            }
        }

        
    }
}
