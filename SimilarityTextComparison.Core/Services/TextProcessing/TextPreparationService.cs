using Microsoft.Extensions.Configuration;
using SimilarityTextComparison.Core.Models.TextProcessing;

namespace SimilarityTextComparison.Core.Services.TextProcessing
{
    public interface ITextPreparationService
    {
        /// <summary>
        /// Prepara il testo dall'input HTML o testo semplice.
        /// </summary>
        /// <param name="inputText">L'input di testo o HTML.</param>
        /// <returns>Lista di token processati.</returns>
        Task<List<Token>> PrepareTextAsync(string inputText);
    }

    /// <summary>
    /// Classe responsabile dell'intero processo di preparazione del testo, inclusa la lettura, pulizia e tokenizzazione.
    /// </summary>
    public class TextPreparationService : ITextPreparationService
    {
        private readonly IConfiguration _config;
        private readonly ITextInputReader _textInputReader;
        private readonly ITextProcessor _textProcessor;
        private readonly ITokenizer _tokenizer;

        public TextPreparationService(
            IConfiguration config,
            ITextInputReader textInputReader,
            ITextProcessor textProcessor,
            ITokenizer tokenizer)
        {
            _config = config;
            _textInputReader = textInputReader;
            _textProcessor = textProcessor;
            _tokenizer = tokenizer;
        }

        /// <summary>
        /// Prepara il testo dall'input HTML o testo semplice.
        /// </summary>
        /// <param name="inputText">L'input di testo o HTML.</param>
        /// <returns>Lista di token processati.</returns>
        public async Task<List<Token>> PrepareTextAsync(string inputText)
        {
            string cleanedText = await _textInputReader.ReadTextInputAsync(inputText);
            string processedText = _textProcessor.CleanText(cleanedText);
            List<Token> tokens = _tokenizer.Tokenize(processedText);
            return tokens;
        }
    }
}