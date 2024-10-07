using System.Text.RegularExpressions;
using ChatGPT_Splitter_Blazor_New.Pages.TextComparer.Model.Comparison;
using ChatGPT_Splitter_Blazor_New.Pages.TextComparer.Model.TextProcessing;
using Match = ChatGPT_Splitter_Blazor_New.Pages.TextComparer.Model.Comparison.Match;

namespace ChatGPT_Splitter_Blazor_New.Pages.TextComparer.Services;

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

    public async Task<List<MatchSegment>> CompareAsync(List<MyInputText> MyInputTexts)
    {
        var forwardReferences = new List<int>();
        var similarities = new List<MatchSegment>();

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

    private List<MatchSegment> _applyStyles(List<MatchSegment> matches)
    {
        // Ordina i match per posizione
        var sortedMatches = matches.OrderBy(m => m.SourceStart).ToList();

        // Applica identificatori unici per l'evidenziazione
        int highlightId = 1;
        foreach (var match in sortedMatches)
        {
            match.HighlightId = highlightId++;
        }

        return sortedMatches;
    }

    private void _readInput(List<MyInputText> MyInputTexts, List<int> forwardReferences)
    {
        int textIndex = 0;
        foreach (var MyInputText in MyInputTexts)
        {
            _tokenizeInput(MyInputText.Text, textIndex);
            textIndex++;
        }
    }

    private void _tokenizeInput(string MyInputText)
    {
        var wordRegex = new Regex(@"\S+"); // Equivalent to [^\s]+
        var matches = wordRegex.Matches(MyInputText);

        foreach (Match match in matches)
        {
            var word = match.Value;
            var cleanedWord = _cleanWord(word);

            if (cleanedWord.Length > 0)
            {
                var token = new Token(cleanedWord, match.Index, match.Index + word.Length);
                Tokens.Add(token);
            }
        }
    }

    private string _cleanWord(string word)
    {
        if (ReplaceUmlaut)
        {
            word = word.Replace("ä", "ae")
                .Replace("ö", "oe")
                .Replace("ü", "ue")
                .Replace("ß", "ss");
        }

        if (IgnorePunctuation)
        {
            word = Regex.Replace(word, @"\p{P}", "");
        }

        if (IgnoreNumbers)
        {
            word = Regex.Replace(word, @"\d", "");
        }

        if (IgnoreLetterCase)
        {
            word = word.ToLower();
        }

        return word;
    }

    private List<MatchSegment> _getSimilarities(int srcTxtIdx, int trgTxtIdx, List<int> forwardReferences)
    {
        var similarities = new List<MatchSegment>();

        var sourceTokens = Tokens.Where(t => t.TextIndex == srcTxtIdx).ToList();
        var targetTokens = Tokens.Where(t => t.TextIndex == trgTxtIdx).ToList();

        // Algoritmo di confronto
        for (int i = 0; i < sourceTokens.Count; i++)
        {
            for (int j = 0; j < targetTokens.Count; j++)
            {
                if (sourceTokens[i].CleanedWord == targetTokens[j].CleanedWord)
                {
                    // Crea un nuovo match
                    var match = new MatchSegment
                    {
                        SourceStart = sourceTokens[i].StartIndex,
                        SourceEnd = sourceTokens[i].EndIndex,
                        TargetStart = targetTokens[j].StartIndex,
                        TargetEnd = targetTokens[j].EndIndex,
                        Length = 1 // Puoi aumentare la logica per lunghezze maggiori
                    };
                    similarities.Add(match);
                }
            }
        }

        return similarities;
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
}