using SimilarityTextComparison.Domain.Models.TextPreProcessing;

namespace SimilarityTextComparison.TestProject;

using Xunit;
using Moq;
using System.Collections.Generic;
using SimilarityTextComparison.Domain.Services.Matching;
using SimilarityTextComparison.Domain.Services.TextPreProcessing;
using SimilarityTextComparison.Infrastructure.Services;

public class ForwardReferenceManagerTests
{
    private readonly Mock<IStorageService> _mockStorage;
    private readonly TextComparisonConfiguration _config;
    private readonly TextProcessor _textProcessor;
    private readonly ForwardReferenceManager _forwardReferenceManager;
    private readonly Matcher _matcher;

    public ForwardReferenceManagerTests()
    {
        // Setup del mock per IStorageService
        _mockStorage = new Mock<IStorageService>();
        _mockStorage.Setup(s => s.GetItemAsync<bool>("ignoreLetterCase")).ReturnsAsync(true);
        _mockStorage.Setup(s => s.GetItemAsync<bool>("ignoreNumbers")).ReturnsAsync(true);
        _mockStorage.Setup(s => s.GetItemAsync<bool>("ignorePunctuation")).ReturnsAsync(true);
        _mockStorage.Setup(s => s.GetItemAsync<bool>("replaceUmlaut")).ReturnsAsync(true);
        _mockStorage.Setup(s => s.GetItemAsync<int>("minMatchLength")).ReturnsAsync(2);
        _mockStorage.Setup(s => s.GetItemAsync<bool>("isHtmlInput")).ReturnsAsync(false);

        // Inizializzazione della configurazione
        _config = new TextComparisonConfiguration(_mockStorage.Object);
        _config.InitializeAsync().Wait();

        // Inizializzazione dei servizi dipendenti
        _textProcessor = new TextProcessor(_config);
        _forwardReferenceManager = new ForwardReferenceManager(_config);
        _matcher = new Matcher(_config);
    }

    [Fact]
    public void Configuration_ShouldBeInitializedCorrectly()
    {
        Assert.True(_config.IgnoreLetterCase);
        Assert.True(_config.IgnoreNumbers);
        Assert.True(_config.IgnorePunctuation);
        Assert.True(_config.ReplaceUmlaut);
        Assert.Equal(2, _config.MinMatchLength);
        Assert.False(_config.IsHtmlInput);
    }

    [Fact]
    public void ForwardReferenceManager_ShouldCreateCorrectReferences()
    {
        // Arrange
        var allTokens = new List<Token>
        {
            new Token("the", 0, 3),
            new Token("quick", 4, 9),
            new Token("brown", 10, 15),
            new Token("fox", 16, 19),
            new Token("the", 20, 23),
            new Token("quick", 24, 29)
        };
        var texts = new List<ProcessedText>
        {
            new ProcessedText(0, 4, allTokens.GetRange(0,4)), // "the quick brown fox"
            new ProcessedText(4, 6, allTokens.GetRange(4,2))  // "the quick"
        };

        // Act
        var forwardRefs = _forwardReferenceManager.CreateForwardReferences(allTokens, texts);

        // Assert
        Assert.Contains(forwardRefs, fr => fr.From == 0 && fr.To == 4); // "the" repeats
        Assert.Contains(forwardRefs, fr => fr.From == 1 && fr.To == 5); // "quick" repeats
    }

    [Fact]
    public void Matcher_ShouldFindCorrectMatches()
    {
        // Arrange
        var sourceText = new ProcessedText(0, 4, new List<Token>
        {
            new Token("the", 0, 3),
            new Token("quick", 4, 9),
            new Token("brown", 10, 15),
            new Token("fox", 16, 19)
        });

        var targetText = new ProcessedText(4, 6, new List<Token>
        {
            new Token("the", 20, 23),
            new Token("quick", 24, 29)
        });

        var allTokens = new List<Token>();
        allTokens.AddRange(sourceText.Tokens);
        allTokens.AddRange(targetText.Tokens);

        var texts = new List<ProcessedText> { sourceText, targetText };
        var forwardReferences = _forwardReferenceManager.CreateForwardReferences(allTokens, texts);

        // Act
        var matches = _matcher.FindMatches(
            sourceTextIndex: 0,
            targetTextIndex: 1,
            sourceText: sourceText,
            targetText: targetText,
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
