using System.Text.RegularExpressions;
using ChatGPT_Splitter_Blazor_New.Pages.TextComparer.Model.Comparison;
using ChatGPT_Splitter_Blazor_New.Pages.TextComparer.Model.TextProcessing;

namespace ChatGPT_Splitter_Blazor_New.Pages.TextComparer.Services
{
    public class SimTexter
    {
        public bool IgnoreLetterCase { get; set; }
        public bool IgnoreNumbers { get; set; }
        public bool IgnorePunctuation { get; set; }
        public bool ReplaceUmlaut { get; set; }
        public int MinMatchLength { get; set; }

        public List<Text> Texts { get; private set; }
        public List<Token> Tokens { get; private set; }
        public int UniqueMatches { get; private set; }

        public SimTexter(StorageService storage)
        {
            IgnoreLetterCase = storage.GetItemValueByKey<bool>("ignoreLetterCase");
            IgnoreNumbers = storage.GetItemValueByKey<bool>("ignoreNumbers");
            IgnorePunctuation = storage.GetItemValueByKey<bool>("ignorePunctuation");
            ReplaceUmlaut = storage.GetItemValueByKey<bool>("replaceUmlaut");
            MinMatchLength = storage.GetItemValueByKey<int>("minMatchLength");

            Texts = new List<Text>();
            Tokens = new List<Token> { new Token() };
            UniqueMatches = 0;
        }

        public async Task<List<List<MatchSegment>>> CompareAsync(List<MyInputText> MyInputTexts)
        {
            var forwardReferences = new List<int>();
            var similarities = new List<List<MatchSegment>>();

            _readInput(MyInputTexts, forwardReferences);
            similarities = _getSimilarities(0, 1, forwardReferences);

            if (similarities.Count > 0)
            {
                return _applyStyles(similarities);
            }
            else
            {
                throw new Exception("No similarities found.");
            }
        }

        private List<List<MatchSegment>> _applyStyles(List<List<MatchSegment>> matches)
        {
            var sortedMatches = _sortSimilarities(matches, 1);
            int styleClassCnt = 1;
            var uniqueMatches = new List<List<MatchSegment>>();

            if (sortedMatches.Count > 0)
            {
                var firstMatch = sortedMatches[0];
                firstMatch[0].SetStyleClass(0);
                firstMatch[1].SetStyleClass(0);
                uniqueMatches.Add(firstMatch);

                for (int i = 1; i < sortedMatches.Count; i++)
                {
                    var lastUniqueMatch = uniqueMatches[^1][1];
                    var match = sortedMatches[i][1];

                    if (lastUniqueMatch.TkBeginPos != match.TkBeginPos)
                    {
                        if (lastUniqueMatch.GetTkEndPosition() - 1 < match.TkBeginPos)
                        {
                            var uniqueMatch = sortedMatches[i];
                            uniqueMatch[0].SetStyleClass(styleClassCnt);
                            uniqueMatch[1].SetStyleClass(styleClassCnt);
                            uniqueMatches.Add(uniqueMatch);
                            styleClassCnt++;
                        }
                        else if (lastUniqueMatch.GetTkEndPosition() < match.GetTkEndPosition())
                        {
                            var styleClass = lastUniqueMatch.StyleClass.EndsWith(" overlapping")
                                ? lastUniqueMatch.StyleClass
                                : lastUniqueMatch.StyleClass + " overlapping";

                            uniqueMatches[^1][0].SetStyleClass(styleClass);
                            uniqueMatches[^1][1].SetStyleClass(styleClass);
                            uniqueMatches[^1][1].MatchLength = match.TkBeginPos - lastUniqueMatch.TkBeginPos;

                            var uniqueMatch = sortedMatches[i];
                            uniqueMatch[0].SetStyleClass(styleClass);
                            uniqueMatch[1].SetStyleClass(styleClass);
                            uniqueMatches.Add(uniqueMatch);
                        }
                    }
                }
            }

            UniqueMatches = uniqueMatches.Count;
            return uniqueMatches;
        }

        private void _readInput(List<MyInputText> MyInputTexts, List<int> forwardReferences)
        {
            var mtsHashTable = new Dictionary<string, int>();

            for (int i = 0; i < MyInputTexts.Count; i++)
            {
                var MyInputText = MyInputTexts[i];
                var nrOfWords = Regex.Matches(MyInputText.Text, @"\S+").Count;
                Texts.Add(new Text(MyInputText.Mode, MyInputText.Text.Length, nrOfWords, MyInputText.FileName, Tokens.Count));
                _tokenizeInput(MyInputText.Text);
                Texts[i].TkEndPos = Tokens.Count;
                _makeForwardReferences(Texts[i], forwardReferences, mtsHashTable);
            }
        }

        private void _tokenizeInput(string MyInputText)
        {
            var cleanedText = _cleanMyInputText(MyInputText);
            var matches = Regex.Matches(cleanedText, @"\S+");

            foreach (Match match in matches)
            {
                var word = match.Value;
                var token = _cleanWord(word);

                if (token.Length > 0)
                {
                    Tokens.Add(new Token(token, match.Index, match.Index + word.Length));
                }
            }
        }

        private string _cleanMyInputText(string MyInputText)
        {
            var text = MyInputText;
            var langRegex = _buildRegex();

            if (langRegex != null)
            {
                text = langRegex.Replace(text, " ");
            }

            if (IgnoreLetterCase)
            {
                text = text.ToLower();
            }

            return text;
        }

        private string _cleanWord(string word)
        {
            if (ReplaceUmlaut)
            {
                word = word.Replace("ä", "ae").Replace("ö", "oe").Replace("ü", "ue").Replace("ß", "ss")
                           .Replace("Ä", "AE").Replace("Ö", "OE").Replace("Ü", "UE");
            }

            return word;
        }

        private List<List<MatchSegment>> _getSimilarities(int srcTxtIdx, int trgTxtIdx, List<int> forwardReferences)
        {
            var similarities = new List<List<MatchSegment>>();
            int srcTkPos = Texts[srcTxtIdx].TkBeginPos;
            int srcTkEndPos = Texts[srcTxtIdx].TkEndPos;

            while ((srcTkPos + MinMatchLength) <= srcTkEndPos)
            {
                MyMatch? bestMatch = _getBestMatch(srcTxtIdx, trgTxtIdx, srcTkPos, forwardReferences);

                if (bestMatch != null && bestMatch.MatchLength > 0)
                {
                    similarities.Add(new List<MatchSegment>
                    {
                        new MatchSegment(bestMatch.SrcTxtIdx, bestMatch.SrcTkBeginPos, bestMatch.MatchLength),
                        new MatchSegment(bestMatch.TrgTxtIdx, bestMatch.TrgTkBeginPos, bestMatch.MatchLength)
                    });
                    srcTkPos += bestMatch.MatchLength;
                }
                else
                {
                    srcTkPos++;
                }
            }

            return similarities;
        }

        /// <summary>
        /// Ordina le corrispondenze in base al segmento di origine o di destinazione,
        /// a seconda del valore dell'indice.
        /// </summary>
        /// <param name="matches">L'array di corrispondenze da ordinare</param>
        /// <param name="idx">L'indice dell'array dei MatchSegment objects</param>
        /// <returns>L'array ordinato di corrispondenze</returns>
        private List<List<MatchSegment>> _sortSimilarities(List<List<MatchSegment>> matches, int idx)
        {
            var sortedSims = new List<List<MatchSegment>>(matches);

            sortedSims.Sort((a, b) =>
            {
                var pos = a[idx].TkBeginPos.CompareTo(b[idx].TkBeginPos);
                if (pos != 0)
                {
                    return pos;
                }
                return b[idx].MatchLength.CompareTo(a[idx].MatchLength);
            });

            return sortedSims;
        }

        /// <summary>
        /// Trova la migliore corrispondenza tra i segmenti di testo corrispondenti.
        /// La migliore corrispondenza è considerata quella con la lunghezza maggiore.
        /// </summary>
        /// <param name="matches">Lista di segmenti corrispondenti</param>
        /// <returns>Il segmento corrispondente più lungo, o null se la lista è vuota</returns>
        private MyMatch _getBestMatch(int srcTxtIdx, int trgTxtIdx, int srcTkBeginPos, List<int> forwardReferences)
        {
            MyMatch bestMatch = null;
            int bestMatchLength = 0;

            for (int tkPos = srcTkBeginPos; tkPos > 0 && tkPos < Tokens.Count; tkPos = forwardReferences[tkPos])
            {
                if (tkPos < Texts[trgTxtIdx].TkBeginPos)
                {
                    continue;
                }

                int minMatchLength = (bestMatchLength > 0) ? bestMatchLength + 1 : MinMatchLength;

                var srcTkPos = srcTkBeginPos + minMatchLength - 1;
                var trgTkPos = tkPos + minMatchLength - 1;

                if (srcTkPos < Texts[srcTxtIdx].TkEndPos &&
                    trgTkPos < Texts[trgTxtIdx].TkEndPos &&
                    (srcTkPos + minMatchLength) <= trgTkPos)
                {
                    int cnt = minMatchLength;

                    while (cnt > 0 && Tokens[srcTkPos].Text == Tokens[trgTkPos].Text)
                    {
                        srcTkPos--;
                        trgTkPos--;
                        cnt--;
                    }

                    if (cnt > 0)
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }

                int newMatchLength = minMatchLength;
                srcTkPos = srcTkBeginPos + minMatchLength;
                trgTkPos = tkPos + minMatchLength;

                while (srcTkPos < Texts[srcTxtIdx].TkEndPos &&
                       trgTkPos < Texts[trgTxtIdx].TkEndPos &&
                       (srcTkPos + newMatchLength) < trgTkPos &&
                       Tokens[srcTkPos].Text == Tokens[trgTkPos].Text)
                {
                    srcTkPos++;
                    trgTkPos++;
                    newMatchLength++;
                }

                if (newMatchLength >= MinMatchLength && newMatchLength > bestMatchLength)
                {
                    bestMatchLength = newMatchLength;
                    var bestMatchTkPos = tkPos;
                    bestMatch = new MyMatch(srcTxtIdx, srcTkBeginPos, trgTxtIdx, bestMatchTkPos, bestMatchLength);
                }
            }

            return bestMatch;
        }

        private void _makeForwardReferences(Text text, List<int> forwardReferences, Dictionary<string, int> mtsTags)
        {
            var txtBeginPos = text.TkBeginPos;
            var txtEndPos = text.TkEndPos;

            for (int i = txtBeginPos; (i + MinMatchLength - 1) < txtEndPos; i++)
            {
                var tag = string.Join("", Tokens.Skip(i).Take(MinMatchLength).Select(t => t.Text));

                if (mtsTags.ContainsKey(tag))
                {
                    forwardReferences[mtsTags[tag]] = i;
                }
                mtsTags[tag] = i;
            }
        }

        private Regex _buildRegex()
        {
            var regexPattern = string.Empty;

            if (IgnoreNumbers)
            {
                regexPattern += @"\p{N}";
            }

            if (IgnorePunctuation)
            {
                regexPattern += @"\p{P}";
            }

            return regexPattern.Length > 0 ? new Regex($"[{regexPattern}]", RegexOptions.Compiled) : null;
        }

        // Implementa gli altri metodi come _getBestMatch, _sortSimilarities, etc.
    }
}