using SimilarityTextComparison.Domain.Interfaces.TextProcessing;
using SimilarityTextComparison.Domain.Models.TextPreProcessing;

namespace SimilarityTextComparison.Domain.Services.TextPreProcessing
{
    /// <summary>
    /// Classe responsabile dell'intero processo di preparazione del testo, inclusa la lettura, pulizia e tokenizzazione.
    /// </summary>
    public class TextPreparationService : ITextPreparationService
    {
        private readonly ITextInputReader _textInputReader;
        private readonly ITextProcessor _textProcessor;
        private readonly ITokenizer _tokenizer;

        public TextPreparationService(ITextInputReader textInputReader,
            ITextProcessor textProcessor,
            ITokenizer tokenizer)
        {
            _textInputReader = textInputReader;
            _textProcessor = textProcessor;
            _tokenizer = tokenizer;
        }

        /// <summary>
        /// Prepara il testo dall'input HTML o testo semplice.
        /// </summary>
        public async Task<(ProcessedText processedText, List<Token> tokens)> PreProcessAndTokenizeText(string inputText)
        {
            string cleanedText = await _textInputReader.ReadTextInputAsync(inputText);
            string cleanedTextNormalized = _textProcessor.CleanText(cleanedText);
            List<Token> tokens = _tokenizer.Tokenize(cleanedTextNormalized);

            var textStatistics = new ProcessedText.TextStatistics(cleanedTextNormalized);

            // Crea un oggetto ProcessedText che rappresenta il testo processato
            var processedText = new ProcessedText(cleanedTextNormalized, textStatistics, tokens);

            return (processedText, tokens);
        }
    }
}