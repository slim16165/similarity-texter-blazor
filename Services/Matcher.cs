using ChatGPT_Splitter_Blazor_New.TextComparer.Model;
using ChatGPT_Splitter_Blazor_New.TextComparer.Model.Comparison;
using ChatGPT_Splitter_Blazor_New.TextComparer.Model.TextProcessing;

namespace ChatGPT_Splitter_Blazor_New.TextComparer.Services;

public class Matcher
{
    private readonly Configuration _config;

    public Matcher(Configuration config)
    {
        _config = config;
    }

    public List<List<MatchSegment>> FindMatches(int sourceTextIndex, int targetTextIndex, MyText sourceText, MyText targetText, Dictionary<int, int> forwardReferences, List<Token> tokens)
    {
        var similarities = new List<List<MatchSegment>>();
        int currentPos = sourceText.TokenBeginPos;

        while (IsWithinMatchRange(currentPos, sourceText.TokenEndPos))
        {
            (TokenPosition SourcePosition, TokenPosition TargetPosition) bestMatch = FindOptimalMatch(currentPos, forwardReferences, tokens, targetText);

            if (bestMatch != (null, null))
            {
                similarities.Add(new List<MatchSegment>
                {
                    new MatchSegment(sourceTextIndex, bestMatch.SourcePosition.BeginPosition, bestMatch.SourcePosition.Length),
                    new MatchSegment(targetTextIndex, bestMatch.TargetPosition.BeginPosition, bestMatch.TargetPosition.Length)
                });
                currentPos += bestMatch.SourcePosition.Length;
            }
            else
            {
                currentPos++;
            }
        }

        return similarities;
    }

    private bool IsWithinMatchRange(int currentPos, int endPos)
    {
        return currentPos + _config.MinMatchLength <= endPos;
    }


    private (TokenPosition SourcePosition, TokenPosition TargetPosition) FindOptimalMatch(int sourceTokenBeginPos, Dictionary<int, int> forwardReferences, List<Token> tokens, MyText targetText)
    {
        var potentialMatches = GetPotentialMatches(sourceTokenBeginPos, forwardReferences, targetText.TokenBeginPos);

        (TokenPosition sourcePos, TokenPosition targetPos) bestMatch = (null, null)!;
        int bestMatchLength = 0;

        foreach (var tokenPos in potentialMatches)
        {
            int matchLength = GetMatchLength(sourceTokenBeginPos, tokenPos, tokens);

            if (matchLength >= _config.MinMatchLength && matchLength > bestMatchLength)
            {
                bestMatchLength = matchLength;

                var sourcePos = new TokenPosition(sourceTokenBeginPos, sourceTokenBeginPos + matchLength);
                var targetPos = new TokenPosition(tokenPos, tokenPos + matchLength);

                bestMatch = (sourcePos, targetPos);
            }
        }

        return bestMatch;
    }

    private static IEnumerable<int> GetPotentialMatches(int sourceTokenBeginPos, Dictionary<int, int> forwardReferences, int targetTokenBeginPos)
    {
        var tokenPos = sourceTokenBeginPos;
        var potentialMatches = new List<int>();

        while (forwardReferences.TryGetValue(tokenPos, out int nextTkPos))
        {
            tokenPos = nextTkPos;

            if (tokenPos >= targetTokenBeginPos)
            {
                potentialMatches.Add(tokenPos);
            }
        }

        return potentialMatches;
    }

    private static int GetMatchLength(int srcTkBeginPos, int trgTkBeginPos, List<Token> tokens)
    {
        int matchLength = 0;

        while (srcTkBeginPos + matchLength < tokens.Count &&
               trgTkBeginPos + matchLength < tokens.Count &&
               tokens[srcTkBeginPos + matchLength].Text == tokens[trgTkBeginPos + matchLength].Text)
        {
            matchLength++;
        }

        return matchLength;
    }
}