using SimilarityTextComparison.Domain.Interfaces.Matching;
using SimilarityTextComparison.Domain.Models.Matching;
using SimilarityTextComparison.Domain.Models.TextPreProcessing;
using SimilarityTextComparison.Infrastructure.Services;


namespace SimilarityTextComparison.Domain.Services.Matching;
public class ForwardReferenceManager : IForwardReferenceManager
{
    private readonly TextComparisonConfiguration _configuration;

    public ForwardReferenceManager(TextComparisonConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Crea una lista globale di forward references per tutti i testi.
    /// </summary>
    /// <param name="allTokens">La lista globale di tutti i token.</param>
    /// <param name="texts">La lista di tutti i testi processati.</param>
    /// <returns>Una lista di ForwardReference che contiene i riferimenti globali.</returns>
    public List<ForwardReference> CreateForwardReferences(List<Token> allTokens, List<ProcessedText> texts)
    {
        var forwardReferences = new List<ForwardReference>();
        var mtsHashTable = new Dictionary<string, int>();

        // Genera tutte le sequenze in anticipo
        var allSequences = PartitionTokensIntoSequences(allTokens, _configuration.MinMatchLength);

        foreach (var (sequence, seqActualPosition) in allSequences)
        {
            int? seqPreviousPosition = GetOrCreateSequencePosition(mtsHashTable, sequence, seqActualPosition);

            if (seqPreviousPosition.HasValue)
            {
                // AddForwardReference
                var forwardReference = new ForwardReference(seqPreviousPosition.Value, seqActualPosition, sequence);
                forwardReferences.Add(forwardReference);
            }
        }

        return forwardReferences;
    }

    private static int? GetOrCreateSequencePosition(
        Dictionary<string, int> mtsHashTable,
        string sequence,
        int actualSequencePosition)
    {
        if (mtsHashTable.TryGetValue(sequence, out int prevPos))
        {
            mtsHashTable[sequence] = actualSequencePosition;
            return prevPos;
        }

        mtsHashTable[sequence] = actualSequencePosition;
        return null;
    }


    private IEnumerable<(string Sequence, int Position)> PartitionTokensIntoSequences(List<Token> tokens, int sequenceLength)
    {
        for (int i = 0; i <= tokens.Count - sequenceLength; i++)
        {
            yield return (GenerateTokenSequenceString(tokens, i, sequenceLength), i);
        }
    }


    /// <summary>
    /// Genera una stringa concatenando i testi dei token a partire da una posizione specificata.
    /// </summary>
    /// <param name="tokens">La lista di token.</param>
    /// <param name="startTokenIndex">L'indice del token da cui iniziare.</param>
    /// <param name="count">Il numero di token da includere nella sequenza.</param>
    /// <returns>La stringa concatenata dei token.</returns>
    private static string GenerateTokenSequenceString(List<Token> tokens, int startTokenIndex, int count)
    {
        return string.Concat(tokens.Skip(startTokenIndex).Take(count).Select(token => token.Text));
    }
}
