using SimilarityTextComparison.Application.Interfaces;
using SimilarityTextComparison.Domain.Interfaces.TextProcessing;
using SimilarityTextComparison.Domain.Models.Matching;
using SimilarityTextComparison.Domain.Models.TextPreProcessing;

namespace SimilarityTextComparison.Application.Services;

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

        foreach (var inputText in inputTexts)
        {
            // Pulisce il testo
            var (processedText, tokens) = await _textPreparationService.PreProcessAndTokenizeText(inputText.Text);
            processedTexts.Add(processedText);
            allTokens.AddRange(tokens);
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
        }
        else
        {
            throw new InvalidOperationException("Nessuna similarità trovata.");
        }

        return matchingSegments;
    }

    /// <summary>
    /// Overload del metodo CompareAsync per confrontare esattamente due testi.
    /// </summary>
    /// <param name="text1">Il primo testo da confrontare.</param>
    /// <param name="text2">Il secondo testo da confrontare.</param>
    /// <returns>Lista dei segmenti corrispondenti tra i due testi.</returns>
    public async Task<List<List<MatchSegment>>> CompareAsync(string text1, string text2)
    {
        if (string.IsNullOrWhiteSpace(text1))
            throw new ArgumentException("Il primo testo non può essere nullo o vuoto.", nameof(text1));

        if (string.IsNullOrWhiteSpace(text2))
            throw new ArgumentException("Il secondo testo non può essere nullo o vuoto.", nameof(text2));

        var (processedText1, tokens1) = await _textPreparationService.PreProcessAndTokenizeText(text1);
        var (processedText2, tokens2) = await _textPreparationService.PreProcessAndTokenizeText(text2);

        var context = new MatchingContext
        {
            SourceText = processedText1,
            TargetText = processedText2,
            Tokens = tokens1.Concat(tokens2).ToList()
        };

        var matchingSegments = await _matchingPipeline.ExecuteAsync(context);

        if (matchingSegments.Count == 0)
            throw new InvalidOperationException("Nessuna similarità trovata.");

        return matchingSegments;
    }
}