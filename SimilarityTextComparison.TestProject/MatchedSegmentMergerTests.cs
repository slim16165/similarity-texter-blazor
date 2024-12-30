using Xunit.Abstractions;
using SimilarityTextComparison.Domain.Models.Matching;
using SimilarityTextComparison.Domain.Services.Matching;

namespace SimilarityTextComparison.TestProject
{
    /// <summary>
    /// Classe di test per MatchedSegmentMerger che verifica il corretto funzionamento della fusione dei segmenti di match.
    /// </summary>
    public class MatchedSegmentMergerTests : BaseTest
    {
        private readonly MatchedSegmentMerger _merger;

        /// <summary>
        /// Costruttore della classe di test che inizializza il MatchedSegmentMerger.
        /// </summary>
        /// <param name="output">Helper per l'output dei test.</param>
        public MatchedSegmentMergerTests(ITestOutputHelper output) : base(output)
        {
            // Inizializza MatchedSegmentMerger. Se ha dipendenze, usa i mock appropriati.
            // Supponiamo che MatchedSegmentMerger abbia un costruttore senza parametri. Altrimenti, modifica di conseguenza.
            _merger = new MatchedSegmentMerger();
        }

        [Fact]
        public void MergeSegments_NoOverlapping_MatchesPreserved()
        {
            // Arrange
            var matches = new List<List<MatchSegment>>
            {
                new List<MatchSegment>
                {
                    new MatchSegment(0, 0, 2),
                    new MatchSegment(1, 4, 2)
                },
                new List<MatchSegment>
                {
                    new MatchSegment(0, 3, 2),
                    new MatchSegment(1, 6, 2)
                }
            };

            // Act
            var merged = _merger.MergeSegments(matches);

            // Log Forward References
            Output.WriteLine("Merged Segments:");
            foreach (var m in merged)
            {
                Output.WriteLine($"Source: [Begin: {m[0].BeginPosition}, Length: {m[0].Length}]");
                Output.WriteLine($"Target: [Begin: {m[1].BeginPosition}, Length: {m[1].Length}]");
            }

            // Assert
            Assert.Equal(2, merged.Count);
            Assert.Contains(merged, m =>
                m[0].BeginPosition == 0 && m[0].Length == 2 &&
                m[1].BeginPosition == 4 && m[1].Length == 2);
            Assert.Contains(merged, m =>
                m[0].BeginPosition == 3 && m[0].Length == 2 &&
                m[1].BeginPosition == 6 && m[1].Length == 2);
        }

        [Fact]
        public void MergeSegments_Overlapping_MatchesMergedCorrectly()
        {
            // Arrange
            var matches = new List<List<MatchSegment>>
            {
                new List<MatchSegment>
                {
                    new MatchSegment(0, 0, 3),
                    new MatchSegment(1, 4, 3)
                },
                new List<MatchSegment>
                {
                    new MatchSegment(0, 2, 3), // Sovrappone con il primo match
                    new MatchSegment(1, 6, 3)  // Sovrappone con il primo match
                },
                new List<MatchSegment>
                {
                    new MatchSegment(0, 5, 2),
                    new MatchSegment(1, 8, 2)
                }
            };

            // Act
            var merged = _merger.MergeSegments(matches);

            // Log Forward References
            Output.WriteLine("Merged Segments:");
            foreach (var m in merged)
            {
                Output.WriteLine($"Source: [Begin: {m[0].BeginPosition}, Length: {m[0].Length}]");
                Output.WriteLine($"Target: [Begin: {m[1].BeginPosition}, Length: {m[1].Length}]");
            }

            // Assert
            Assert.Equal(3, merged.Count);
            Assert.Contains(merged, m =>
                m[0].BeginPosition == 0 && m[0].Length == 3 &&
                m[1].BeginPosition == 4 && m[1].Length == 3);
            Assert.Contains(merged, m =>
                m[0].BeginPosition == 2 && m[0].Length == 3 &&
                m[1].BeginPosition == 6 && m[1].Length == 3);
            Assert.Contains(merged, m =>
                m[0].BeginPosition == 5 && m[0].Length == 2 &&
                m[1].BeginPosition == 8 && m[1].Length == 2);
        }
    }
}
