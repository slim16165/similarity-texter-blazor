using SimilarityTextComparison.Domain.Interfaces.Matching;
using SimilarityTextComparison.Domain.Models.Matching;
using SimilarityTextComparison.Domain.Models.Position;
using SimilarityTextComparison.Domain.Models.TextPreProcessing;
using SimilarityTextComparison.Infrastructure.Services;

namespace SimilarityTextComparison.Domain.Services.Matching;

/// <summary>
/// Confronta testi sorgente e destinazione trovando segmenti corrispondenti basati su token, utilizzando le forward references e configurazioni specifiche per determinare le migliori corrispondenze.
/// </summary>
public class Matcher : IMatcher
{
    private readonly Configuration _configuration;

    public Matcher(Configuration configuration)
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
    /// <param name="forwardReferences">I riferimenti avanzati per il testo sorgente.</param>
    /// <param name="tokens">La lista di tutti i token.</param>
    /// <returns>Una lista di liste di segmenti corrispondenti.</returns>
    public List<List<MatchSegment>> FindMatches(
        int sourceTextIndex,
        int targetTextIndex,
        ProcessedText sourceText,
        ProcessedText targetText,
        Dictionary<int, int> forwardReferences,
        List<Token> tokens)
    {
        var matchingSegments = new List<List<MatchSegment>>();
        int currentPosition = 0; // Inizializzato a 0

        // Continua finché ci sono abbastanza token rimanenti per un match di lunghezza minima
        while (IsWithinMatchRange(currentPosition, sourceText.Tokens.Count))
        {
            // Trova il miglior match a partire dalla posizione corrente
            var bestMatch = FindBestMatch(currentPosition, forwardReferences, tokens);

            if (bestMatch is { SourcePosition: not null, TargetPosition: not null })
            {
                // Aggiungi i segmenti corrispondenti alla lista dei risultati
                matchingSegments.Add([
                    new MatchSegment(sourceTextIndex, bestMatch.SourcePosition.BeginPosition,
                        bestMatch.SourcePosition.Length),
                    new MatchSegment(targetTextIndex, bestMatch.TargetPosition.BeginPosition,
                        bestMatch.TargetPosition.Length)
                ]);

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
    /// Verifica se ci sono abbastanza token rimanenti per cercare un match di lunghezza minima.
    /// </summary>
    /// <param name="currentPosition">La posizione corrente nel testo.</param>
    /// <param name="endPosition">La posizione finale nel testo.</param>
    /// <returns>True se ci sono abbastanza token, altrimenti False.</returns>
    private bool IsWithinMatchRange(int currentPosition, int endPosition)
    {
        return currentPosition + _configuration.MinMatchLength <= endPosition;
    }

    /// <summary>
    /// Trova il miglior match possibile a partire da una posizione specifica nel testo sorgente.
    /// </summary>
    /// <param name="sourceTokenStartPos">Posizione iniziale nel testo sorgente.</param>
    /// <param name="forwardReferences">I riferimenti avanzati per il testo sorgente.</param>
    /// <param name="tokens">La lista di tutti i token.</param>
    /// <returns>Una tupla contenente le posizioni dei token del match nel sorgente e nel target.</returns>
    /// <summary>
    /// Trova il miglior match possibile a partire da una posizione specifica nel testo sorgente.
    /// </summary>
    private (TokenPosition SourcePosition, TokenPosition TargetPosition) FindBestMatch(
        int sourceTokenStartPos,
        Dictionary<int, int> forwardReferences,
        List<Token> tokens)
    {
        // Ottiene le posizioni potenziali di match nel testo di destinazione
        var potentialMatchPositions = GetPotentialMatchPositions(sourceTokenStartPos, forwardReferences, 0); // Sostituito con 0

        TokenPosition bestSourcePos = null;
        TokenPosition bestTargetPos = null;
        int bestMatchLength = 0;

        foreach (var targetTokenPos in potentialMatchPositions)
        {
            // Determina la lunghezza del match a partire dalle posizioni date
            int matchLength = GetMatchLength(sourceTokenStartPos, targetTokenPos, tokens);

            if (matchLength >= _configuration.MinMatchLength && matchLength > bestMatchLength)
            {
                bestMatchLength = matchLength;

                bestSourcePos = new TokenPosition(sourceTokenStartPos, sourceTokenStartPos + matchLength);
                bestTargetPos = new TokenPosition(targetTokenPos, targetTokenPos + matchLength);
            }
        }

        return (bestSourcePos, bestTargetPos)!;
    }

    /// <summary>
    /// Ottiene le posizioni potenziali di match nel testo di destinazione per una data posizione nel sorgente.
    /// </summary>
    /// <param name="sourceTokenStartPos">Posizione iniziale nel testo sorgente.</param>
    /// <param name="forwardReferences">I riferimenti avanzati per il testo sorgente.</param>
    /// <param name="targetTokenBeginPos">Posizione iniziale nel testo di destinazione.</param>
    /// <returns>Una collezione di posizioni di token nel testo di destinazione che potrebbero corrispondere.</returns>
    private static IEnumerable<int> GetPotentialMatchPositions(
        int sourceTokenStartPos,
        Dictionary<int, int> forwardReferences,
        int targetTokenBeginPos)
    {
        var tokenPos = sourceTokenStartPos;
        var potentialMatches = new List<int>();

        while (forwardReferences.TryGetValue(tokenPos, out int nextTokenPos))
        {
            tokenPos = nextTokenPos;

            if (tokenPos >= targetTokenBeginPos)
            {
                potentialMatches.Add(tokenPos);
            }
        }

        return potentialMatches;
    }

    /// <summary>
    /// Calcola la lunghezza del match tra il testo sorgente e il testo di destinazione a partire dalle posizioni specificate.
    /// </summary>
    /// <param name="sourceTokenStartPos">Posizione iniziale nel testo sorgente.</param>
    /// <param name="targetTokenStartPos">Posizione iniziale nel testo di destinazione.</param>
    /// <param name="tokens">La lista di tutti i token.</param>
    /// <returns>La lunghezza del match trovato.</returns>
    private static int GetMatchLength(int sourceTokenStartPos, int targetTokenStartPos, List<Token> tokens)
    {
        int matchLength = 0;

        while (sourceTokenStartPos + matchLength < tokens.Count &&
               targetTokenStartPos + matchLength < tokens.Count &&
               tokens[sourceTokenStartPos + matchLength].Text == tokens[targetTokenStartPos + matchLength].Text)
        {
            matchLength++;
        }

        return matchLength;
    }
}