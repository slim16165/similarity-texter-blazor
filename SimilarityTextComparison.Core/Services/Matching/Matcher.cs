using SimilarityTextComparison.Domain.Interfaces.Matching;
using SimilarityTextComparison.Domain.Models.Matching;
using SimilarityTextComparison.Domain.Models.Position;
using SimilarityTextComparison.Domain.Models.Position.Enum;
using SimilarityTextComparison.Domain.Models.TextPreProcessing;
using SimilarityTextComparison.Infrastructure.Services;
using System.Diagnostics;

namespace SimilarityTextComparison.Domain.Services.Matching
{
    public class Matcher : IMatcher
    {
        // Struttura interna per rappresentare un match esteso
        private struct ExtendedMatch
        {
            public TokenPosition SourcePosition { get; set; }
            public TokenPosition TargetPosition { get; set; }
            public int MatchLength { get; set; }

            public override string ToString()
            {
                return $"Source [{SourcePosition.BeginPosition}, {SourcePosition.Length}] <-> " +
                       $"Target [{TargetPosition.BeginPosition}, {TargetPosition.Length}] (Length={MatchLength})";
            }
        }

        private readonly TextComparisonConfiguration _configuration;

        public Matcher(TextComparisonConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Trova i segmenti corrispondenti tra il testo sorgente e il testo di destinazione.
        /// </summary>
        /// <param name="sourceTextIndex">Indice del testo sorgente.</param>
        /// <param name="targetTextIndex">Indice del testo di destinazione.</param>
        /// <param name="sourceText">Il testo sorgente.</param>
        /// <param name="targetText">Il testo di destinazione.</param>
        /// <param name="forwardReferences">La lista globale di forward references.</param>
        /// <param name="tokens">La lista unificata di tutti i token.</param>
        /// <returns>Una lista di liste di segmenti corrispondenti.</returns>
        public List<List<MatchSegment>> FindMatches(
            int sourceTextIndex,
            int targetTextIndex,
            ProcessedText sourceText,
            ProcessedText targetText,
            List<ForwardReference> forwardReferences,
            List<Token> tokens)
        {
            Console.WriteLine("────────────────────────────────────────────");
            Console.WriteLine("→ Avvio ricerca corrispondenze (MatcherStep)");

            // Log dei testi e delle posizioni rilevanti
            Console.WriteLine($"SourceTextIndex={sourceTextIndex}, TargetTextIndex={targetTextIndex}");
            Console.WriteLine($"Sorgente: '{sourceText.Text}'");
            Console.WriteLine($"Target:   '{targetText.Text}'");
            Console.WriteLine($"Analizzeremo i token da {sourceText.TkBeginPos} a {sourceText.TkEndPos - 1} (inclusi).");

            var matchingSegments = new List<List<MatchSegment>>();
            int currentPosition = sourceText.TkBeginPos;

            // Memorizza l'ultimo match per evitare duplicati nella pipeline
            (int start, int length)? lastMatch = null;

            // Ciclo di ricerca dei match
            while (IsWithinMatchRange(currentPosition, sourceText.TkEndPos))
            {
                // Trova il miglior match a questa posizione
                var bestMatch = FindBestMatchAtPosition(
                    currentPosition,
                    forwardReferences,
                    sourceText,
                    targetText,
                    tokens
                );

                if (bestMatch.SourcePosition != null && bestMatch.TargetPosition != null)
                {
                    var matchStart = bestMatch.SourcePosition.BeginPosition;
                    var matchLen = bestMatch.SourcePosition.Length;

                    // Evita di registrare lo stesso match più volte
                    if (lastMatch.HasValue && lastMatch.Value.start == matchStart && lastMatch.Value.length == matchLen)
                    {
                        // Avanza di un token per prevenire loop infinito
                        currentPosition++;
                    }
                    else
                    {
                        // Creazione e aggiunta del match
                        var matchPair = CreateMatchPair(
                            sourceTextIndex,
                            targetTextIndex,
                            bestMatch,
                            tokens
                        );

                        matchingSegments.Add(matchPair);

                        // Log di riepilogo match trovato
                        Console.WriteLine($"→ Match trovato! PosSorgente={matchStart} Lunghezza={bestMatch.MatchLength}");
                        Console.WriteLine(
                            $"   Sorgente: [{bestMatch.SourcePosition.BeginPosition}, {bestMatch.SourcePosition.Length}] " +
                            $"\"{GetSequenceText(bestMatch.SourcePosition, tokens)}\"\n" +
                            $"   Target:   [{bestMatch.TargetPosition.BeginPosition}, {bestMatch.TargetPosition.Length}] " +
                            $"\"{GetSequenceText(bestMatch.TargetPosition, tokens)}\""
                        );

                        // Salva l'ultimo match
                        lastMatch = (matchStart, matchLen);

                        // Avanza di 1 token dall'inizio del match
                        currentPosition = matchStart + 1;
                    }
                }
                else
                {
                    // Nessun match su questa posizione
                    Console.WriteLine($"   Nessun match a pos={currentPosition}, avanzo di 1 token.");
                    currentPosition++;
                    lastMatch = null;
                }
            }

            Console.WriteLine($"→ Ricerca completata. Trovati {matchingSegments.Count} match totali.");
            Console.WriteLine("────────────────────────────────────────────\n");
            return matchingSegments;
        }


        /// <summary>
        /// Verifica se possiamo ancora cercare un match a partire dalla posizione corrente,
        /// in base al limite di tokens e alla minMatchLength configurata.
        /// </summary>
        private bool IsWithinMatchRange(int currentPos, int endPos)
        {
            return currentPos + _configuration.MinMatchLength <= endPos;
        }

        /// <summary>
        /// Individua il miglior match tra quelli candidati a partire dalla posizione indicata.
        /// </summary>
        private ExtendedMatch FindBestMatchAtPosition(
            int sourceStartPos,
            List<ForwardReference> forwardReferences,
            ProcessedText sourceText,
            ProcessedText targetText,
            List<Token> tokens)
        {
            // Pesca solo le forward references rilevanti per la posizione attuale
            var relevantReferences = FilterRelevantForwardReferences(
                sourceStartPos,
                forwardReferences,
                sourceText,
                targetText
            );

            // Inizializzazione per il "miglior match"
            TokenPosition bestSourcePos = null;
            TokenPosition bestTargetPos = null;
            int bestMatchLength = 0;

            foreach (var fr in relevantReferences)
            {
                int initialMatchLength = GetMatchLength(fr.From, fr.To, tokens);

                // Se la lunghezza iniziale è al di sotto del minimo, log e skip
                if (initialMatchLength < _configuration.MinMatchLength)
                {
                    Console.WriteLine(
                        $"   Scartata sequenza '{fr.Sequence}' " +
                        $"(From={fr.From}, To={fr.To}, initLen={initialMatchLength}) " +
                        $"poiché < MinMatchLength={_configuration.MinMatchLength}."
                    );
                    continue;
                }

                // Prova a estendere la corrispondenza
                var extendedMatch = ExtendMatch(fr, sourceText, targetText, tokens);

                // Se miglior match di quanto trovato finora, aggiorna
                if (extendedMatch.MatchLength > bestMatchLength)
                {
                    bestMatchLength = extendedMatch.MatchLength;
                    bestSourcePos = extendedMatch.SourcePosition;
                    bestTargetPos = extendedMatch.TargetPosition;
                }
            }

            // Se ho references rilevanti ma il best match = 0
            if (bestMatchLength == 0 && relevantReferences.Any())
            {
                Console.WriteLine(
                    $"   Trovate forward references, ma match=0 da pos={sourceStartPos}. " +
                    $"Possibile anomalia."
                );
                Debugger.Break(); // breakpoint di debug per analizzare la situazione
            }

            return new ExtendedMatch
            {
                SourcePosition = bestSourcePos,
                TargetPosition = bestTargetPos,
                MatchLength = bestMatchLength
            };
        }

        /// <summary>
        /// Filtra le forward references che partono esattamente dalla posizione
        /// sorgente indicata e che rientrano nei limiti di sorgente e target.
        /// </summary>
        private static List<ForwardReference> FilterRelevantForwardReferences(
            int sourceStartPos,
            List<ForwardReference> forwardReferences,
            ProcessedText sourceText,
            ProcessedText targetText)
        {
            var relevantRefs = forwardReferences.Where(fr =>
                fr.From >= sourceText.TkBeginPos && fr.From < sourceText.TkEndPos &&
                fr.To >= targetText.TkBeginPos && fr.To < targetText.TkEndPos &&
                fr.From == sourceStartPos
            ).ToList();

            if (!relevantRefs.Any())
            {
                Console.WriteLine($"   Nessuna forward reference valida a pos={sourceStartPos}.");
            }

            return relevantRefs;
        }

        /// <summary>
        /// Allunga il match all'indietro e in avanti, se i token corrispondono.
        /// </summary>
        private ExtendedMatch ExtendMatch(
            ForwardReference forwardReference,
            ProcessedText sourceText,
            ProcessedText targetText,
            List<Token> tokens)
        {
            int srcPos = forwardReference.From;
            int trgPos = forwardReference.To;

            // Calcolo match "grezzo"
            int matchLen = GetMatchLength(srcPos, trgPos, tokens);

            // Estende il match all'indietro
            while (srcPos > sourceText.TkBeginPos &&
                   trgPos > targetText.TkBeginPos &&
                   tokens[srcPos - 1].Text == tokens[trgPos - 1].Text)
            {
                srcPos--;
                trgPos--;
                matchLen++;
            }

            // Estende il match in avanti
            while (srcPos + matchLen < sourceText.TkEndPos &&
                   trgPos + matchLen < targetText.TkEndPos &&
                   tokens[srcPos + matchLen].Text == tokens[trgPos + matchLen].Text)
            {
                matchLen++;
            }

            // Se il match finale resta sotto la soglia: log di eventuale anomalia
            if (matchLen < _configuration.MinMatchLength)
            {
                Console.WriteLine(
                    $"   Match esteso < soglia minima ({matchLen} < {_configuration.MinMatchLength})."
                );
                Debugger.Break();
            }

            return new ExtendedMatch
            {
                SourcePosition = new TokenPosition(srcPos, srcPos + matchLen),
                TargetPosition = new TokenPosition(trgPos, trgPos + matchLen),
                MatchLength = matchLen
            };
        }

        /// <summary>
        /// Crea effettivamente la coppia di MatchSegment (sorgente e target).
        /// </summary>
        private List<MatchSegment> CreateMatchPair(
            int sourceTextIndex,
            int targetTextIndex,
            ExtendedMatch match,
            List<Token> tokens)
        {
            var sourceMatch = new MatchSegment(
                sourceTextIndex,
                match.SourcePosition.BeginPosition,
                match.SourcePosition.Length
            )
            {
                MatchedText = match.SourcePosition.RetrieveMatchedText(tokens)
            };

            var targetMatch = new MatchSegment(
                targetTextIndex,
                match.TargetPosition.BeginPosition,
                match.TargetPosition.Length
            )
            {
                MatchedText = match.TargetPosition.RetrieveMatchedText(tokens)
            };

            return new List<MatchSegment> { sourceMatch, targetMatch };
        }

        /// <summary>
        /// Calcola la lunghezza del match a partire dalle due posizioni corrispondenti in tokens.
        /// </summary>
        private static int GetMatchLength(int srcStart, int trgStart, List<Token> tokens)
        {
            int length = 0;
            while (srcStart + length < tokens.Count &&
                   trgStart + length < tokens.Count &&
                   tokens[srcStart + length].Text == tokens[trgStart + length].Text)
            {
                length++;
            }
            return length;
        }

        /// <summary>
        /// Recupera la sottostringa di token corrispondente a un determinato TokenPosition.
        /// </summary>
        private static string GetSequenceText(TokenPosition position, List<Token> tokens)
        {
            return string.Join(" ",
                tokens.Skip(position.BeginPosition)
                      .Take(position.Length)
                      .Select(t => t.Text));
        }
    }
}