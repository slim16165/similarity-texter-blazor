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
        // Sorting and styling logic goes here.
        return matches; // Return matches with applied styles.
    }

    private void _readInput(List<MyInputText> MyInputTexts, List<int> forwardReferences)
    {
        foreach (var MyInputText in MyInputTexts)
        {
            // Tokenize input text and store in Tokens list.
            _tokenizeInput(MyInputText.Text);
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
        // Apply cleaning rules based on settings.
        return word.ToLower();
    }

    private List<MatchSegment> _getSimilarities(int srcTxtIdx, int trgTxtIdx, List<int> forwardReferences)
    {
        var similarities = new List<MatchSegment>();
        // Logic to find similarities and matches between texts.
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