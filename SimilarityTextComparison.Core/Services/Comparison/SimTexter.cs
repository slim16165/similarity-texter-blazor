using SimilarityTextComparison.Core.Interfaces;
using SimilarityTextComparison.Core.Models.Comparison;
using SimilarityTextComparison.Core.Models.TextProcessing;
using SimilarityTextComparison.Core.Services.TextProcessing;

namespace SimilarityTextComparison.Core.Services.Comparison;

public class SimTexter : ISimTexter
{
    private readonly Configuration.Configuration _configuration;
    private readonly Tokenizer _tokenizer;
    private readonly ForwardReferenceManager _forwardReferenceManager;
    private readonly Matcher _matcher;
    private readonly StyleApplier _styleApplier;

    public SimTexter(IStorageService storageService = null)
    {
        _configuration = new Configuration.Configuration(storageService ?? throw new ArgumentNullException(nameof(storageService)));
        _tokenizer = new Tokenizer(_configuration);
        _forwardReferenceManager = new ForwardReferenceManager(_configuration);
        _matcher = new Matcher(_configuration);
        _styleApplier = new StyleApplier();
    }

    /// <summary>
    /// Confronta una lista di testi di input e restituisce i segmenti corrispondenti trovati.
    /// </summary>
    /// <param name="inputTexts">Lista dei testi di input da confrontare.</param>
    /// <returns>Lista dei segmenti corrispondenti tra i testi.</returns>
    /// <exception cref="ArgumentException">Se il numero di testi di input è inferiore a 2.</exception>
    public async Task<List<List<MatchSegment>>> CompareAsync(List<MyInputText> inputTexts)
    {
        // Verifica che ci siano almeno due testi da confrontare
        if (inputTexts == null || inputTexts.Count < 2)
            throw new ArgumentException("Sono necessari almeno due testi per il confronto.", nameof(inputTexts));

        // Processa i testi di input: pulizia e tokenizzazione
        var processedTexts = PreprocessInputTexts(inputTexts);

        // Crea i riferimenti avanzati per il primo testo per ottimizzare il processo di matching
        var forwardReferences = _forwardReferenceManager.CreateForwardReferences(processedTexts[0]);

        // Trova i segmenti corrispondenti tra il primo e il secondo testo
        var matchingSegments = _matcher.FindMatches(
            sourceTextIndex: 0,
            targetTextIndex: 1,
            sourceText: processedTexts[0],
            targetText: processedTexts[1],
            forwardReferences: forwardReferences,
            tokens: Tokenizer.GlobalTokens);

        // Verifica se sono stati trovati segmenti corrispondenti
        if (matchingSegments.Count > 0)
        {
            // Applica gli stili ai segmenti corrispondenti per la visualizzazione
            return _styleApplier.ApplyStyles(matchingSegments);
        }
        else
        {
            throw new InvalidOperationException("Nessuna similarità trovata.");
        }
    }

    /// <summary>
    /// Preprocessa i testi di input effettuando la pulizia e la tokenizzazione.
    /// </summary>
    /// <param name="inputTexts">Lista dei testi di input.</param>
    /// <returns>Una tupla contenente la lista dei testi processati e la lista di tutti i token.</returns>
    public List<MyText> PreprocessInputTexts(List<MyInputText> inputTexts)
    {
        var processedTexts = new List<MyText>();
        var allTokens = new List<Token>();
        int currentTokenPosition = 0;

        foreach (var inputText in inputTexts)
        {
            var inputInfo = new InputInfo(inputText.Mode, inputText.FileName);
            var textStatistics = new TextStatistics(inputText.Text);

            // Pulisce il testo rimuovendo caratteri indesiderati secondo le configurazioni
            string cleanedText = new TextProcessor(_configuration).CleanText(inputText.Text);

            // Tokenizza il testo pulito
            var tokens = _tokenizer.Tokenize(cleanedText);

            // Crea un oggetto MyText che rappresenta il testo processato
            var tokenizationInfo = new TokenizationInfo(currentTokenPosition, tokens.Count);
            var processedText = new MyText(cleanedText, inputInfo, textStatistics, tokenizationInfo);

            processedTexts.Add(processedText);
            allTokens.AddRange(tokens);
            currentTokenPosition += tokens.Count;
        }

        Tokenizer.GlobalTokens = allTokens;

        return processedTexts;
    }
}