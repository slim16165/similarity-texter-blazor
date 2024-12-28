using SimilarityTextComparison.Domain.Models.TextPreProcessing;
using SimilarityTextComparison.Domain.Services.Matching;
using SimilarityTextComparison.Domain.Services.TextPreProcessing;
using SimilarityTextComparison.Infrastructure.Services;

namespace SimilarityTextComparison.TestProject;

public class MatcherTests
{
    [Fact]
    public void FindMatches_SimpleText_CorrectMatches()
    {
        // Arrange
        var config = new TextComparisonConfiguration
        {
            MinMatchLength = 2,
            IgnoreLetterCase = true,
            IgnoreNumbers = true,
            IgnorePunctuation = true,
            ReplaceUmlaut = true
        };
        var textProcessor = new TextProcessor(config);

        // Testo Sorgente: "the quick brown fox the quick"
        var sourceTextString = "the quick brown fox the quick";
        var sourceCleaned = textProcessor.CleanText(sourceTextString);
        var sourceTokens = Tokenize(sourceCleaned, 0);

        // Testo Target: "jumps over the lazy dog the quick"
        var targetTextString = "jumps over the lazy dog the quick";
        var targetCleaned = textProcessor.CleanText(targetTextString);
        var targetTokens = Tokenize(targetCleaned, sourceTokens.Count);

        var sourceProcessedText = new ProcessedText
        {
            Tokens = sourceTokens,
            TkBeginPos = 0,
            TkEndPos = sourceTokens.Count
        };

        var targetProcessedText = new ProcessedText
        {
            Tokens = targetTokens,
            TkBeginPos = sourceTokens.Count,
            TkEndPos = sourceTokens.Count + targetTokens.Count
        };

        var allTokens = new List<Token>();
        allTokens.AddRange(sourceTokens);
        allTokens.AddRange(targetTokens);
        var texts = new List<ProcessedText> { sourceProcessedText, targetProcessedText };

        var forwardReferenceManager = new ForwardReferenceManager(config);
        var forwardReferences = forwardReferenceManager.CreateForwardReferences(allTokens, texts);

        var matcher = new Matcher(config);

        // Act
        var matches = matcher.FindMatches(
            sourceTextIndex: 0,
            targetTextIndex: 1,
            sourceText: sourceProcessedText,
            targetText: targetProcessedText,
            forwardReferences: forwardReferences,
            tokens: allTokens
        );

        // Assert
        Assert.Single(matches); // Solo una corrispondenza "the quick"
        var matchPair = matches[0];
        Assert.Equal(0, matchPair[0].TextIndex);
        Assert.Equal(0, matchPair[0].BeginPosition);
        Assert.Equal(2, matchPair[0].Length);

        Assert.Equal(1, matchPair[1].TextIndex);
        Assert.Equal(4, matchPair[1].BeginPosition);
        Assert.Equal(2, matchPair[1].Length);
    }

    private List<Token> Tokenize(string text, int startIndex)
    {
        var tokens = new List<Token>();
        var words = text.Split(' ');
        int pos = startIndex;

        foreach (var word in words)
        {
            if (string.IsNullOrWhiteSpace(word))
                continue;

            var begin = pos;
            var end = pos + word.Length;
            tokens.Add(new Token(word, begin, end));
            pos = end + 1; // +1 per lo spazio
        }

        return tokens;
    }
}