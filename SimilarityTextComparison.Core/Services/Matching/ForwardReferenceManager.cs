using SimilarityTextComparison.Domain.Interfaces.Matching;
using SimilarityTextComparison.Domain.Models.TextPreProcessing;
using SimilarityTextComparison.Domain.Services.TextPreProcessing;
using SimilarityTextComparison.Infrastructure.Services;

namespace SimilarityTextComparison.Domain.Services.Matching;

/// <summary>
/// Ottimizza il processo di matching creando riferimenti avanzati (forward references) per identificare rapidamente ripetizioni di sequenze di token nel testo.
/// </summary>
public class ForwardReferenceManager : IForwardReferenceManager
{
    private readonly TextComparisonConfiguration _configuration;

    public ForwardReferenceManager(TextComparisonConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Crea un dizionario di riferimenti avanzati (forward references) per ottimizzare il matching.
    /// Per ogni sequenza di token ripetuta nel testo, viene creata una forward reference che punta
    /// alla successiva occorrenza della stessa sequenza. Questo permette di saltare rapidamente alle
    /// ripetizioni e migliorare l'efficienza del matching.
    ///
    /// **Esempio di forward reference:**
    /// - Se la sequenza "cat sat" appare alla posizione 1 e si ripete alla posizione 6,
    ///   viene creato un riferimento: 1 → 6.
    /// </summary>
    /// <param name="text">Il testo da analizzare.</param>
    /// <returns>Un dizionario che mappa la posizione iniziale della sequenza alla posizione successiva.</returns>
    public Dictionary<int, int> CreateForwardReferences(ProcessedText text)
    {
        // The tokenSequencePositions dictionary tracks the positions of token sequences within the text.
        // This is used to update the forward references by storing the last seen position of each sequence.
        // When a repeated sequence is found, its previous position is retrieved from this dictionary to create a forward reference pointing to the current position.
        var tokenSequencePositions = new Dictionary<string, int>();
        var forwardReferences = new Dictionary<int, int>();

        //Usa una sliding window
        for (int i = 0; i + _configuration.MinMatchLength - 1 < text.Tokens.Count; i++)
        {
            string tokenSequence = GenerateTokenSequenceString(i, _configuration.MinMatchLength);
            int? previousPosition = FindPreviousPosition(tokenSequence, tokenSequencePositions);

            if (previousPosition.HasValue)
            {
                CreateForwardReference(forwardReferences, previousPosition.Value, i);
            }

            UpdateCurrentPosition(tokenSequencePositions, tokenSequence, i);
        }

        return forwardReferences;
    }

    /// <summary>
    /// Finds the previous position of the token sequence.
    /// </summary>
    private static int? FindPreviousPosition(string tokenSequence, Dictionary<string, int> tokenSequencePositions)
    {
        return tokenSequencePositions.ContainsKey(tokenSequence)
            ? tokenSequencePositions[tokenSequence]
            : null;
    }

    /// <summary>
    /// Updates the current position of the token sequence in the dictionary.
    /// </summary>
    private static void UpdateCurrentPosition(Dictionary<string, int> tokenSequencePositions, string tokenSequence, int currentPosition)
    {
        tokenSequencePositions[tokenSequence] = currentPosition;
    }

    /// <summary>
    /// Creates a forward reference from the previous position to the current position.
    /// </summary>
    private static void CreateForwardReference(Dictionary<int, int> forwardReferences, int previousPosition, int currentPosition)
    {
        forwardReferences[previousPosition] = currentPosition;
    }

    /// <summary>
    /// Genera una stringa concatenando i testi dei token a partire da una posizione specificata.
    /// </summary>
    private static string GenerateTokenSequenceString(int startTokenIndex, int count)
    {
        var tokenSequence = Tokenizer.GlobalTokens
            .Skip(startTokenIndex)
            .Take(count);

        return string.Concat(tokenSequence.Select(token => token.Text));
    }

}