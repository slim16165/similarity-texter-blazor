using SimilarityTextComparison.Domain.Models.TextPreProcessing;
using Xunit.Abstractions;

namespace SimilarityTextComparison.TestProject;

/// <summary>
/// Classe di test per ForwardReferenceManager.
/// </summary>
public class ForwardReferenceManagerTests : BaseTest
{
    public ForwardReferenceManagerTests(ITestOutputHelper output) : base(output) { }

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
    public void ForwardReferenceManager_ShouldCreateCorrectReferences()
    {
        // Arrange
        var allTokens = CreateTokens(new List<string> { "the", "quick", "brown", "fox", "the", "quick" });

        var sourceText = CreateProcessedText("the quick brown fox", allTokens, 0, 4);
        var targetText = CreateProcessedText("the quick", allTokens, 4, 2);

        var texts = new List<ProcessedText> { sourceText, targetText };

        // Act
        var forwardRefs = ForwardReferenceManager.CreateForwardReferences(allTokens, texts);

        // Log Forward References
        Output.WriteLine("Forward References:");
        foreach (var fr in forwardRefs)
        {
            Output.WriteLine($"From: {fr.FromTokenPos}, To: {fr.ToTokenPos}, Sequence: {fr.Sequence}");
        }

        // Assert
        Assert.Single(forwardRefs); // Assicurati che ci sia solo una ForwardReference

        var fr1 = forwardRefs[0];
        Assert.Equal(0, fr1.FromTokenPos);
        Assert.Equal(4, fr1.ToTokenPos);
        Assert.Equal("the quick", fr1.Sequence);
    }
}