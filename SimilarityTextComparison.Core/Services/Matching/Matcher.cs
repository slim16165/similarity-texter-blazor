using SimilarityTextComparison.Domain.Interfaces.Matching;
using SimilarityTextComparison.Domain.Models.Matching;
using SimilarityTextComparison.Domain.Models.Position;
using SimilarityTextComparison.Domain.Models.TextPreProcessing;
using SimilarityTextComparison.Infrastructure.Services;

namespace SimilarityTextComparison.Domain.Services.Matching;

public class Matcher : IMatcher
{
    private readonly TextComparisonConfiguration _configuration;

    public Matcher(TextComparisonConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Trova i segmenti corrispondenti tra il testo sorgente e il testo di destinazione.
    /// </summary>
    /// <param name="sourceTextIndex">Indice del testo sorgente.</param>
    /// <param name="targetTextIndex">Indice del testo di destinazione.</param>
    /// <param name="sourceText">Il testo sorgente.</param>
    /// <param name="targetText">Il testo di destinazione.</param>
    /// <param name="forwardReferences">La lista globale di forward references.</param>
    /// <param name="tokens">La lista di tutti i token.</param>
    /// <returns>Una lista di liste di segmenti corrispondenti.</returns>
    public List<List<MatchSegment>> FindMatches(
        int sourceTextIndex,
        int targetTextIndex,
        ProcessedText sourceText,
        ProcessedText targetText,
        List<ForwardReference> forwardReferences,
        List<Token> tokens)
    {
        var matchingSegments = new List<List<MatchSegment>>();
        int currentPosition = sourceText.TkBeginPos; // Inizializzato alla posizione di inizio del sorgente

        // Continua finché ci sono abbastanza token rimanenti nel sorgente per un match di lunghezza minima
        while (IsWithinMatchRange(currentPosition, sourceText.TkEndPos))
        {
            // Trova il miglior match a partire dalla posizione corrente
            var bestMatch = FindBestMatch(currentPosition, forwardReferences, sourceText, targetText, tokens);

            if (bestMatch.SourcePosition != null && bestMatch.TargetPosition != null)
            {
                // Crea una nuova lista di MatchSegment
                var matchSegmentPair = new List<MatchSegment>
                {
                    new MatchSegment(sourceTextIndex, bestMatch.SourcePosition.BeginPosition, bestMatch.SourcePosition.Length),
                    new MatchSegment(targetTextIndex, bestMatch.TargetPosition.BeginPosition, bestMatch.TargetPosition.Length)
                };

                // Aggiungi i segmenti corrispondenti alla lista dei risultati
                matchingSegments.Add(matchSegmentPair);

                // Avanza la posizione corrente di lunghezza del match trovato
                currentPosition += bestMatch.SourcePosition.Length;
            }
            else
            {
                // Nessun match trovato, avanza di un token
                currentPosition++;
            }
        }

        return matchingSegments;
    }

    /// <summary>
    /// Verifica se ci sono abbastanza token rimanenti nel sorgente per cercare un match di lunghezza minima.
    /// </summary>
    private bool IsWithinMatchRange(int currentPosition, int endPosition)
    {
        return currentPosition + _configuration.MinMatchLength <= endPosition;
    }

    /// <summary>
    /// Trova il miglior match possibile a partire da una posizione specifica nel testo sorgente.
    /// </summary>
    private (TokenPosition SourcePosition, TokenPosition TargetPosition) FindBestMatch(
        int sourceTokenStartPos,
        List<ForwardReference> forwardReferences,
        ProcessedText sourceText,
        ProcessedText targetText,
        List<Token> tokens)
    {
        // Filtra le forward references dove 'from' è nel sorgente e 'to' è nel target
        var relevantReferences = forwardReferences.Where(fr =>
            fr.From >= sourceText.TkBeginPos && fr.From < sourceText.TkEndPos &&
            fr.To >= targetText.TkBeginPos && fr.To < targetText.TkEndPos
        ).ToList();

        TokenPosition bestSourcePos = null;
        TokenPosition bestTargetPos = null;
        int bestMatchLength = 0;

        foreach (var fr in relevantReferences)
        {
            int targetTokenPos = fr.To;
            int matchLength = GetMatchLength(sourceTokenStartPos, targetTokenPos, tokens);

            if (matchLength >= _configuration.MinMatchLength && matchLength > bestMatchLength)
            {
                bestMatchLength = matchLength;
                bestSourcePos = new TokenPosition(sourceTokenStartPos, sourceTokenStartPos + matchLength);
                bestTargetPos = new TokenPosition(targetTokenPos, targetTokenPos + matchLength);
            }
        }

        if (bestSourcePos != null && bestTargetPos != null)
        {
            return (bestSourcePos, bestTargetPos);
        }

        return default;
    }

    /// <summary>
    /// Calcola la lunghezza del match tra il testo sorgente e il testo di destinazione a partire dalle posizioni specificate.
    /// </summary>
    private static int GetMatchLength(int sourceTokenStartPos, int targetTokenStartPos, List<Token> tokens)
    {
        int matchLength = 0;

        // Confronta i token finché corrispondono
        while (sourceTokenStartPos + matchLength < tokens.Count &&
               targetTokenStartPos + matchLength < tokens.Count &&
               tokens[sourceTokenStartPos + matchLength].Text == tokens[targetTokenStartPos + matchLength].Text)
        {
            matchLength++;
        }

        return matchLength;
    }
}