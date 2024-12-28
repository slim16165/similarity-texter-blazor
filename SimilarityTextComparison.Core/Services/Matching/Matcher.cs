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

    public List<List<MatchSegment>> FindMatches(
            int sourceTextIndex,
            int targetTextIndex,
            ProcessedText sourceText,
            ProcessedText targetText,
            List<ForwardReference> forwardReferences,
            List<Token> tokens)
    {
        var matchingSegments = new List<List<MatchSegment>>();
        int currentPosition = sourceText.TkBeginPos;

        while (IsWithinMatchRange(currentPosition, sourceText.TkEndPos))
        {
            var best = FindBestMatch(forwardReferences, sourceText, targetText, tokens);
            if (best is { SourcePosition: not null, TargetPosition: not null })
            {
                // Crea i due segmenti (sorgente e target) con indici globali
                var matchPair = new List<MatchSegment>
                    {
                        new MatchSegment(sourceTextIndex, best.SourcePosition.BeginPosition, best.SourcePosition.Length),
                        new MatchSegment(targetTextIndex, best.TargetPosition.BeginPosition, best.TargetPosition.Length)
                    };

                matchingSegments.Add(matchPair);

                // Avanza la posizione
                currentPosition = best.SourcePosition.EndPosition;
            }
            else
            {
                currentPosition++;
            }
        }

        return matchingSegments;
    }

    private bool IsWithinMatchRange(int currentPos, int endPos)
    {
        return currentPos + _configuration.MinMatchLength <= endPos;
    }

    private (TokenPosition SourcePosition, TokenPosition TargetPosition) FindBestMatch(
        List<ForwardReference> forwardReferences,
        ProcessedText sourceText,
        ProcessedText targetText,
        List<Token> tokens)
    {
        var relevantRefs = forwardReferences.Where(fr =>
            fr.FromTokenPos >= sourceText.TkBeginPos && fr.FromTokenPos < sourceText.TkEndPos &&
            fr.ToTokenPos >= targetText.TkBeginPos && fr.ToTokenPos < targetText.TkEndPos
        ).ToList();

        TokenPosition bestSrcPos = null;
        TokenPosition bestTrgPos = null;
        int bestLen = 0;

        foreach (var fr in relevantRefs)
        {
            // Esempio di match iniziale
            int initialLength = GetMatchLength(fr.FromTokenPos, fr.ToTokenPos, tokens);
            if (initialLength < _configuration.MinMatchLength) continue;

            // Iniziamo con la parte comune
            int srcPos = fr.FromTokenPos;
            int trgPos = fr.ToTokenPos;
            int matchLen = initialLength;

            // Se la posizione da cui parti non coincide con sourceStartPos,
            // puoi testare anche quell'allineamento. In questo esempio
            // usiamo i forwardReferences come “ipotesi di match”.

            // STEP 1: Estendi all'indietro
            while (srcPos > sourceText.TkBeginPos && trgPos > targetText.TkBeginPos
                   && tokens[srcPos - 1].Text == tokens[trgPos - 1].Text)
            {
                srcPos--;
                trgPos--;
                matchLen++;
            }

            // STEP 2: Estendi in avanti
            while (srcPos + matchLen < sourceText.TkEndPos
                   && trgPos + matchLen < targetText.TkEndPos
                   && tokens[srcPos + matchLen].Text == tokens[trgPos + matchLen].Text)
            {
                matchLen++;
            }

            if (matchLen >= _configuration.MinMatchLength && matchLen > bestLen)
            {
                bestLen = matchLen;
                bestSrcPos = new TokenPosition(srcPos, srcPos + matchLen);
                bestTrgPos = new TokenPosition(trgPos, trgPos + matchLen);
            }
        }

        return (bestSrcPos, bestTrgPos);
    }

    private int GetMatchLength(int srcStart, int trgStart, List<Token> tokens)
    {
        int length = 0;
        while (srcStart + length < tokens.Count
               && trgStart + length < tokens.Count
               && tokens[srcStart + length].Text == tokens[trgStart + length].Text)
        {
            length++;
        }
        return length;
    }
}