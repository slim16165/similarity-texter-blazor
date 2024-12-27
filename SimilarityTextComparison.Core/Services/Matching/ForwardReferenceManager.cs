using SimilarityTextComparison.Domain.Interfaces.Matching;
using SimilarityTextComparison.Domain.Models.TextPreProcessing;
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
    /// </summary>
    /// <param name="text">Il testo da analizzare.</param>
    /// <returns>Un dizionario che mappa la posizione iniziale della sequenza alla posizione successiva.</returns>
    public Dictionary<int, int> CreateForwardReferences(ProcessedText text)
    {
        var tokenSequencePositions = new Dictionary<string, int>();
        var forwardReferences = new Dictionary<int, int>();

        // Usa una sliding window
        for (int i = 0; i + _configuration.MinMatchLength - 1 < text.Tokens.Count; i++)
        {
            string tokenSequence = GenerateTokenSequenceString(text.Tokens, i, _configuration.MinMatchLength);
            if (tokenSequencePositions.TryGetValue(tokenSequence, out int previousPosition))
            {
                forwardReferences[previousPosition] = i;
            }

            tokenSequencePositions[tokenSequence] = i;
        }

        return forwardReferences;
    }

    /// <summary>
    /// Genera una stringa concatenando i testi dei token a partire da una posizione specificata.
    /// </summary>
    private static string GenerateTokenSequenceString(List<Token> tokens, int startTokenIndex, int count)
    {
        return string.Concat(tokens.Skip(startTokenIndex).Take(count).Select(token => token.Text));
    }
}