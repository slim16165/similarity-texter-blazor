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
                       $"Target [{TargetPosition.BeginPosition}, {TargetPosition.Length}] (Length: {MatchLength})";
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
            var matchingSegments = new List<List<MatchSegment>>();
            int currentPosition = sourceText.TkBeginPos;

            Console.WriteLine("Inizio ricerca dei match.");
            Console.WriteLine($"Testo Sorgente: '{sourceText.Text}'");
            Console.WriteLine($"Testo Target: '{targetText.Text}'");
            Console.WriteLine($"Starting matching from position {currentPosition} to {sourceText.TkEndPos}");


            // Memorizza l'ultimo match per evitare ripetizioni
            (int start, int length)? lastMatch = null;

            while (IsWithinMatchRange(currentPosition, sourceText.TkEndPos))
            {
                var bestMatch =
                    FindBestMatchAtPosition(currentPosition, forwardReferences, sourceText, targetText, tokens);

                if (bestMatch.SourcePosition != null && bestMatch.TargetPosition != null)
                {
                    var matchStart = bestMatch.SourcePosition.BeginPosition;
                    var matchLen = bestMatch.SourcePosition.Length;

                    // Verifica se il match corrente è identico al precedente
                    if (lastMatch.HasValue && lastMatch.Value.start == matchStart && lastMatch.Value.length == matchLen)
                    {
                        // Avanza di 1 token per evitare loop infinito
                        currentPosition++;
                    }
                    else
                    {
                        // Crea e aggiungi il match alla lista
                        var matchPair = CreateMatchPair(sourceTextIndex, targetTextIndex, bestMatch);
                        matchingSegments.Add(matchPair);

                        Console.WriteLine($"Match found at source {bestMatch.SourcePosition.BeginPosition} with length {bestMatch.MatchLength}");

                        Console.WriteLine(
                            $"Match trovato: Source [{bestMatch.SourcePosition.BeginPosition}, {bestMatch.SourcePosition.Length}] '{GetSequenceText(bestMatch.SourcePosition, tokens)}' <-> " +
                            $"Target [{bestMatch.TargetPosition.BeginPosition}, {bestMatch.TargetPosition.Length}] '{GetSequenceText(bestMatch.TargetPosition, tokens)}' (Length: {bestMatch.MatchLength})");

                        // Aggiorna l'ultimo match trovato
                        lastMatch = (matchStart, matchLen);

                        // Avanza di 1 token dall'inizio del match per permettere la scoperta di match sovrapposti
                        currentPosition = matchStart + 1;
                    }
                }
                else
                {
                    Console.WriteLine(
                        $"Nessun match trovato alla posizione {currentPosition}. Avanzamento di un token.");
                    currentPosition++;
                    lastMatch = null; // Resetta l'ultimo match poiché non c'è stato un match
                }
            }

            Console.WriteLine($"Ricerca dei match completata. Totale match trovati: {matchingSegments.Count}");
            return matchingSegments;
        }



        /// <summary>
        /// Verifica se la posizione corrente è entro il range per un possibile match.
        /// </summary>
        private bool IsWithinMatchRange(int currentPos, int endPos)
        {
            return currentPos + _configuration.MinMatchLength <= endPos;
        }

        /// <summary>
        /// Trova il miglior match possibile a partire da una posizione specifica nel testo sorgente.
        /// </summary>
        private ExtendedMatch FindBestMatchAtPosition(
            int sourceStartPos,
            List<ForwardReference> forwardReferences,
            ProcessedText sourceText,
            ProcessedText targetText,
            List<Token> tokens)
        {
            var relevantReferences = FilterRelevantForwardReferences(sourceStartPos, forwardReferences, sourceText, targetText);

            TokenPosition bestSourcePos = null;
            TokenPosition bestTargetPos = null;
            int bestMatchLength = 0;

            foreach (var fr in relevantReferences)
            {
                int initialMatchLength = GetMatchLength(fr.From, fr.To, tokens);

                if (initialMatchLength < _configuration.MinMatchLength)
                {
                    Console.WriteLine($"Sequenza '{fr.Sequence}' alla posizione From: {fr.From}, To: {fr.To} ignorata (lunghezza iniziale {initialMatchLength} < MinMatchLength {_configuration.MinMatchLength}).");
                    continue;
                }

                var extendedMatch = ExtendMatch(fr, sourceText, targetText, tokens);

                if (extendedMatch.MatchLength > bestMatchLength)
                {
                    bestMatchLength = extendedMatch.MatchLength;
                    bestSourcePos = extendedMatch.SourcePosition;
                    bestTargetPos = extendedMatch.TargetPosition;

                    Console.WriteLine($"Nuovo best match trovato: {extendedMatch}");
                }
            }

            // Condizione inattesa: nessun match trovato ma ci si aspetta almeno uno
            if (bestMatchLength == 0 && relevantReferences.Any())
            {
                Console.WriteLine($"Match trovato con lunghezza 0 per le forward references rilevanti a partire da {sourceStartPos}.");
                Debugger.Break();
            }

            return new ExtendedMatch
            {
                SourcePosition = bestSourcePos,
                TargetPosition = bestTargetPos,
                MatchLength = bestMatchLength
            };
        }

        private static string GetSequenceText(TokenPosition position, List<Token> tokens)
        {
            var sequenceTokens = tokens
                .Skip(position.BeginPosition)
                .Take(position.Length)
                .Select(t => t.Text);

            return string.Join(" ", sequenceTokens);
        }


        /// <summary>
        /// Filtra le forward references rilevanti per l'attuale posizione nel sorgente e target.
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
                fr.From == sourceStartPos // Associa solo le forward references che iniziano alla posizione corrente
            ).ToList();

            if (!relevantRefs.Any())
            {
                Console.WriteLine($"Nessuna forward reference rilevante trovata per la posizione {sourceStartPos}.");
            }

            return relevantRefs;
        }

        /// <summary>
        /// Estende un match all'indietro e in avanti per massimizzare la lunghezza del match.
        /// </summary>
        private ExtendedMatch ExtendMatch(
            ForwardReference forwardReference,
            ProcessedText sourceText,
            ProcessedText targetText,
            List<Token> tokens)
        {
            int srcPos = forwardReference.From;
            int trgPos = forwardReference.To;
            int matchLen = GetMatchLength(srcPos, trgPos, tokens);

            // Estensione all'indietro
            while (srcPos > sourceText.TkBeginPos &&
                   trgPos > targetText.TkBeginPos &&
                   tokens[srcPos - 1].Text == tokens[trgPos - 1].Text)
            {
                srcPos--;
                trgPos--;
                matchLen++;
            }

            // Estensione in avanti
            while (srcPos + matchLen < sourceText.TkEndPos &&
                   trgPos + matchLen < targetText.TkEndPos &&
                   tokens[srcPos + matchLen].Text == tokens[trgPos + matchLen].Text)
            {
                matchLen++;
            }

            // Controllo inatteso: matchLen non incrementato
            if (matchLen < _configuration.MinMatchLength)
            {
                Console.WriteLine($"Match esteso ha lunghezza inferiore al minimo: {matchLen} < {_configuration.MinMatchLength}.");
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
        /// Crea una coppia di MatchSegment per il match trovato.
        /// </summary>
        private static List<MatchSegment> CreateMatchPair(
            int sourceTextIndex,
            int targetTextIndex,
            ExtendedMatch match)
        {
            if (match.SourcePosition == null || match.TargetPosition == null)
            {
                Console.WriteLine("ExtendedMatch contiene posizioni null.");
                Debugger.Break();
            }

            return new List<MatchSegment>
            {
                new MatchSegment(sourceTextIndex, match.SourcePosition.BeginPosition, match.SourcePosition.Length),
                new MatchSegment(targetTextIndex, match.TargetPosition.BeginPosition, match.TargetPosition.Length)
            };
        }




        /// <summary>
        /// Calcola la lunghezza del match tra il testo sorgente e il testo di destinazione a partire dalle posizioni specificate.
        /// </summary>
        private static int GetMatchLength(int srcStart, int trgStart, List<Token> tokens)
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
}
