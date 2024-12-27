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

        foreach (var text in texts)
        {
            for (int i = text.TkBeginPos; i < text.TkEndPos; i++)
            {
                // Genera la sequenza di token
                var tokenSequence = GenerateTokenSequenceString(allTokens, i, _configuration.MinMatchLength);

                if (mtsHashTable.TryGetValue(tokenSequence, out int previousPosition))
                {
                    forwardReferences.Add(new ForwardReference(previousPosition, i));
                }

                mtsHashTable[tokenSequence] = i;
            }
        }

        return forwardReferences;
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
