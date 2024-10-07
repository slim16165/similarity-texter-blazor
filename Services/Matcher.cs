using System.Collections.Generic;
using ChatGPT_Splitter_Blazor_New.Pages.TextComparer.Model.TextProcessing;
using ChatGPT_Splitter_Blazor_New.TextComparer.Model.Comparison;
using ChatGPT_Splitter_Blazor_New.TextComparer.Model.TextProcessing;

namespace ChatGPT_Splitter_Blazor_New.TextComparer.Services
{
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
            int currentPos = sourceText.TokenkBeginPos;

            while (IsWithinMatchRange(currentPos, sourceText.TokenEndPos))
            {
                var bestMatch = FindOptimalMatch(sourceTextIndex, targetTextIndex, currentPos, forwardReferences, tokens, targetText);

                if (bestMatch != null)
                {
                    similarities.Add(CreateMatchSegments(bestMatch));
                    currentPos += bestMatch.MatchLength;
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
            return (currentPos + _config.MinMatchLength) <= endPos;
        }


        private MyMatch? FindOptimalMatch(int sourceTextIndex, int targetTextIndex, int sourceTokenBeginPos, Dictionary<int, int> forwardReferences, List<Token> tokens, MyText targetText)
        {
            MyMatch? bestMatch = null;
            int bestMatchLength = 0;
            int tkPos = sourceTokenBeginPos;

            while (forwardReferences.TryGetValue(tkPos, out int nextTkPos))
            {
                tkPos = nextTkPos;

                if (tkPos < targetText.TokenkBeginPos)
                    continue;

                int minMatchLength = (bestMatchLength > 0) ? bestMatchLength + 1 : _config.MinMatchLength;

                if (IsValidInitialMatch(sourceTokenBeginPos, tkPos, minMatchLength, tokens))
                {
                    int newMatchLength = CalculateMatchLength(sourceTokenBeginPos, tkPos, minMatchLength, tokens);

                    if (newMatchLength >= _config.MinMatchLength && newMatchLength > bestMatchLength)
                    {
                        bestMatchLength = newMatchLength;
                        bestMatch = new MyMatch(sourceTextIndex, sourceTokenBeginPos, targetTextIndex, tkPos, newMatchLength);
                    }
                }
            }

            return bestMatch;
        }

        private static bool IsValidInitialMatch(int srcTkBeginPos, int trgTkBeginPos, int minMatchLength, List<Token> tokens)
        {
            int srcTkPos = srcTkBeginPos + minMatchLength - 1;
            int trgTkPos = trgTkBeginPos + minMatchLength - 1;

            if (srcTkPos >= tokens.Count || trgTkPos >= tokens.Count)
                return false;

            for (int i = 0; i < minMatchLength; i++)
            {
                if (tokens[srcTkBeginPos + i].Text != tokens[trgTkBeginPos + i].Text)
                    return false;
            }

            return true;
        }

        private static int CalculateMatchLength(int srcTkBeginPos, int trgTkBeginPos, int initialMatchLength, List<Token> tokens)
        {
            int matchLength = initialMatchLength;

            while (true)
            {
                int srcIndex = srcTkBeginPos + matchLength;
                int trgIndex = trgTkBeginPos + matchLength;

                if (srcIndex >= tokens.Count || trgIndex >= tokens.Count ||
                    tokens[srcIndex].Text != tokens[trgIndex].Text)
                {
                    break;
                }

                matchLength++;
            }

            return matchLength;
        }

        private static List<MatchSegment> CreateMatchSegments(MyMatch bestMatch)
        {
            return new List<MatchSegment>
            {
                new MatchSegment(bestMatch.SrcTxtIdx, bestMatch.SrcTkBeginPos, bestMatch.MatchLength),
                new MatchSegment(bestMatch.TrgTxtIdx, bestMatch.TrgTkBeginPos, bestMatch.MatchLength)
            };
        }
    }
}