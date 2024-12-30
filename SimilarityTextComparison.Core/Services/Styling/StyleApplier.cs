using SimilarityTextComparison.Domain.Interfaces.Styling;
using SimilarityTextComparison.Domain.Models.Matching;

namespace SimilarityTextComparison.Domain.Services.Styling
{
    /// <summary>
    /// Classe responsabile di applicare stili (classi CSS, ad esempio)
    /// ai segmenti di match, gestendo eventuali sovrapposizioni.
    /// </summary>
    public class StyleApplier : IStyleApplier
    {
        /// <summary>
        /// Numero di match unici a cui è stato applicato uno stile.
        /// </summary>
        public int UniqueMatches { get; private set; }

        /// <summary>
        /// Applica stili a una lista di liste di <see cref="MatchSegment"/>,
        /// gestendo sovrapposizioni e contatori di stile.
        /// </summary>
        /// <param name="matches">Lista di match, ognuno composto da coppie (o più segmenti).</param>
        /// <returns>Una lista di match con stili applicati.</returns>
        public List<List<MatchSegment>> ApplyStyles(List<List<MatchSegment>> matches)
        {
            Console.WriteLine("────────────────────────────────────────────");
            Console.WriteLine("→ Inizio ApplyStyles in StyleApplier");

            if (matches == null || matches.Count == 0)
            {
                Console.WriteLine("   Nessun match da processare, restituisco lista vuota.");
                Console.WriteLine("────────────────────────────────────────────\n");
                return new List<List<MatchSegment>>();
            }

            Console.WriteLine($"   Numero di match in ingresso: {matches.Count}");

            var styleClassCount = 1;
            var uniqueMatches = new List<List<MatchSegment>>();

            // itera su ogni set/coppia di match
            foreach (var matchPair in matches)
            {
                AssignStyleToMatch(matchPair, uniqueMatches, ref styleClassCount);
            }

            UniqueMatches = uniqueMatches.Count;

            Console.WriteLine($"→ Stili applicati. UniqueMatches={UniqueMatches}");
            Console.WriteLine("────────────────────────────────────────────\n");

            return uniqueMatches;
        }

        /// <summary>
        /// Metodo principale che gestisce l'applicazione dello stile a un nuovo match,
        /// controllando se è non-sovrapposto o in overlap.
        /// </summary>
        private static void AssignStyleToMatch(
            List<MatchSegment> currentMatch,
            List<List<MatchSegment>> uniqueMatches,
            ref int styleClassCount)
        {
            if (currentMatch == null || currentMatch.Count < 2)
            {
                Console.WriteLine("   [Warning] currentMatch nullo o non valido, skip.");
                return;
            }

            // Se è il primo match, assegna e vai
            if (!uniqueMatches.Any())
            {
                Console.WriteLine("   Primo match: assegno stile '0' per entrambe le parti.");
                InitializeFirstMatchStyle(currentMatch, uniqueMatches);
                return;
            }

            // Prende l'ultimo match nella lista e confronta i segmenti target (index=1)
            var lastUniqueMatch = uniqueMatches.Last()[1];
            var current = currentMatch[1];

            // Se non si sovrappongono, applichiamo uno stile nuovo
            if (lastUniqueMatch.IsNonOverlapping(current))
            {
                Console.WriteLine($"   Nuovo match non sovrapposto, assegno styleClass={styleClassCount}.");
                ApplyNewStyle(currentMatch, styleClassCount, uniqueMatches);
                styleClassCount++;
            }
            // Se c’è sovrapposizione potenziale
            else if (lastUniqueMatch.CanExtendOverlap(current))
            {
                Console.WriteLine("   Trovato overlap: estendo stile di overlap.");
                lastUniqueMatch.ExtendOverlapStyle(current);
                // Aggiunge la coppia ai match unici
                uniqueMatches.Add(currentMatch);
            }
            else
            {
                // Caso in cui non è “non-sovrapposto” e non “estendibile”
                // ma potrebbe essercene uno di parziale sovrapposizione o conflitto.
                Console.WriteLine("   Overlap parziale o situazione non standard, assegno stile nuovo e aggiungo.");
                ApplyNewStyle(currentMatch, styleClassCount, uniqueMatches);
                styleClassCount++;
            }
        }

        /// <summary>
        /// Assegna un nuovo stile ai due segmenti e aggiunge la coppia alla lista dei match unici.
        /// </summary>
        private static void ApplyNewStyle(
            List<MatchSegment> currentMatch,
            int styleClassCount,
            List<List<MatchSegment>> uniqueMatches)
        {
            if (currentMatch.Count < 2) return;

            currentMatch[0].SetStyleClass(styleClassCount);
            currentMatch[1].SetStyleClass(styleClassCount);

            uniqueMatches.Add(currentMatch);
        }

        /// <summary>
        /// Inizializza il primo match assegnandogli una classe di stile fissa (es: 0),
        /// e lo aggiunge ai match unici.
        /// </summary>
        private static void InitializeFirstMatchStyle(
            List<MatchSegment> firstMatch,
            List<List<MatchSegment>> uniqueMatches)
        {
            if (firstMatch.Count < 2) return;

            firstMatch[0].SetStyleClass(0);
            firstMatch[1].SetStyleClass(0);

            uniqueMatches.Add(firstMatch);
        }
    }
}