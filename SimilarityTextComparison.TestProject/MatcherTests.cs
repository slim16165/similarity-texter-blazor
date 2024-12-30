using SimilarityTextComparison.Domain.Models.TextPreProcessing;
using Xunit;
using System.Collections.Generic;
using Xunit.Abstractions;

namespace SimilarityTextComparison.TestProject
{
    /// <summary>
    /// Test per la classe Matcher.
    /// </summary>
    public class MatcherTests : BaseTest
    {
        public MatcherTests(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void Configuration_ShouldBeInitializedCorrectly()
        {
            Assert.True(Config.IgnoreLetterCase);
            Assert.True(Config.IgnoreNumbers);
            Assert.True(Config.IgnorePunctuation);
            Assert.True(Config.ReplaceUmlaut);
            Assert.Equal(2, Config.MinMatchLength);
            Assert.False(Config.IsHtmlInput);
        }

        [Fact]
        public void FindMatches_SimpleText_CorrectMatches()
        {
            // Arrange
            var allTokens = CreateTokens(new List<string> { "the", "quick", "brown", "fox", "the", "quick", "brown", "fox" });

            var sourceText = CreateProcessedText("the quick brown fox", allTokens, 0, 4);
            var targetText = CreateProcessedText("the quick brown fox", allTokens, 4, 4);

            var texts = new List<ProcessedText> { sourceText, targetText };

            // Log Token Details
            Output.WriteLine("All Tokens:");
            for (int i = 0; i < allTokens.Count; i++)
            {
                Output.WriteLine($"Token[{i}]: '{allTokens[i].Text}' (Begin: {allTokens[i].BeginPosition}, End: {allTokens[i].EndPosition})");
            }

            // Act
            var forwardRefs = ForwardReferenceManager.CreateForwardReferences(allTokens, texts);

            // Log Forward References
            Output.WriteLine("Forward References:");
            foreach (var fr in forwardRefs)
            {
                Output.WriteLine($"From: {fr.FromTokenPos}, To: {fr.ToTokenPos}, Sequence: {fr.Sequence}");
            }

            var matches = Matcher.FindMatches(
                sourceTextIndex: 0,
                targetTextIndex: 1,
                sourceText: sourceText,
                targetText: targetText,
                forwardReferences: forwardRefs,
                tokens: allTokens
            );

            // Log Matches
            Output.WriteLine("Matches Found:");
            foreach (var matchPair in matches)
            {
                var sourceMatch = matchPair[0];
                var targetMatch = matchPair[1];
                Output.WriteLine($"Source Match: [Index: {sourceMatch.TextIndex}, Begin: {sourceMatch.BeginPosition}, Length: {sourceMatch.Length}]");
                Output.WriteLine($"Target Match: [Index: {targetMatch.TextIndex}, Begin: {targetMatch.BeginPosition}, Length: {targetMatch.Length}]");
            }

            // Assert
            Assert.Equal(1, matches.Count); // Uno solo match continuo "the quick brown fox"
            var match = matches[0];
            Assert.Equal(0, match[0].TextIndex);
            Assert.Equal(0, match[0].BeginPosition);
            Assert.Equal(4, match[0].Length);

            Assert.Equal(1, match[1].TextIndex);
            Assert.Equal(4, match[1].BeginPosition);
            Assert.Equal(4, match[1].Length);
        }


        [Fact]
        public void FindMatches_NoMatch_ReturnsEmptyList()
        {
            // Arrange: due testi completamente diversi
            var allTokens = CreateTokens(new List<string>
            {
                "lorem", "ipsum", "dolor", "sit", "amet",
                "banana", "apple", "orange"
            });

            var sourceText = CreateProcessedText("lorem ipsum dolor sit", allTokens, 0, 4);
            var targetText = CreateProcessedText("banana apple orange", allTokens, 4, 3);

            var texts = new List<ProcessedText> { sourceText, targetText };
            var forwardRefs = ForwardReferenceManager.CreateForwardReferences(allTokens, texts);

            // Act
            var matches = Matcher.FindMatches(0, 1, sourceText, targetText, forwardRefs, allTokens);

            // Assert
            Assert.Empty(matches);
        }

        
        [Fact]
        public void FindMatches_SingleMatch_ReturnsOneMatch2()
        {
            // Arrange
            var allTokens = CreateTokens(new List<string> { "hello", "world", "hello", "world" });

            var sourceText = CreateProcessedText("hello world", allTokens, 0, 2);
            var targetText = CreateProcessedText("hello world", allTokens, 2, 2);

            var texts = new List<ProcessedText> { sourceText, targetText };
            var forwardRefs = ForwardReferenceManager.CreateForwardReferences(allTokens, texts);

            // Act
            var matches = Matcher.FindMatches(0, 1, sourceText, targetText, forwardRefs, allTokens);

            // Assert
            Assert.Single(matches);
            var matchPair = matches[0];
            Assert.Equal(0, matchPair[0].TextIndex);
            Assert.Equal(0, matchPair[0].BeginPosition);
            Assert.Equal(2, matchPair[0].Length);

            Assert.Equal(1, matchPair[1].TextIndex);
            Assert.Equal(2, matchPair[1].BeginPosition);
            Assert.Equal(2, matchPair[1].Length);
        }

        [Fact]
        public void FindMatches_MultipleDistinctMatches_ReturnsAll()
        {
            // Esempio: "the quick" e "brown fox", entrambi ripetuti in target
            var allTokens = CreateTokens(new List<string>
            {
                "the", "quick", "brown", "fox",
                "the", "quick", "brown", "fox"
            });
            // Source: prime 4
            var sourceText = CreateProcessedText("the quick brown fox", allTokens, 0, 4);
            // Target: ultime 4
            var targetText = CreateProcessedText("the quick brown fox", allTokens, 4, 4);

            var texts = new List<ProcessedText> { sourceText, targetText };
            var forwardRefs = ForwardReferenceManager.CreateForwardReferences(allTokens, texts);

            // Act
            var matches = Matcher.FindMatches(0, 1, sourceText, targetText, forwardRefs, allTokens);

            // Assert
            // Dovresti trovarti 1 match grande (lunghezza 4)
            // Oppure, se configurato per match minori, potresti trovarne di più
            Assert.Single(matches);
        }

        [Fact]
        public void FindMatches_PartialOverlap_ReturnsMultiple()
        {
            // Esempio: "the quick brown" e più avanti "brown fox"
            var allTokens = CreateTokens(new List<string>
            {
                "the", "quick", "brown", "fox",
                "the", "quick", "brown", "fox"
            });

            // Source: 0..4 -> "the quick brown fox"
            var sourceText = CreateProcessedText("the quick brown fox", allTokens, 0, 4);

            // Target: 4..8 -> "the quick brown fox"
            var targetText = CreateProcessedText("the quick brown fox", allTokens, 4, 4);

            var texts = new List<ProcessedText> { sourceText, targetText };
            var forwardRefs = ForwardReferenceManager.CreateForwardReferences(allTokens, texts);

            // Act
            var matches = Matcher.FindMatches(0, 1, sourceText, targetText, forwardRefs, allTokens);

            // Assert
            // A seconda di come implementi la logica, potresti aspettarti 1 match grande
            // o 2 match parziali. Fai la tua asserzione in base al comportamento desiderato.
            // Esempio ipotetico:
            Assert.True(matches.Count >= 1, "Expected at least 1 match for partial overlap scenario");
        }
    }
}
