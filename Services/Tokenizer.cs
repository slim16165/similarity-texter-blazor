using System.Text.RegularExpressions;
using ChatGPT_Splitter_Blazor_New.Pages.TextComparer.Model.TextProcessing;
using ChatGPT_Splitter_Blazor_New.TextComparer.Model.TextProcessing;

namespace ChatGPT_Splitter_Blazor_New.TextComparer.Services;

public class Tokenizer
{
    private readonly Configuration _config;

    public Tokenizer(Configuration config)
    {
        _config = config;
    }

    public List<Token> Tokenize(string text)
    {
        var tokens = new List<Token>();
        var matches = Regex.Matches(text, @"\S+");

        foreach (Match match in matches)
        {
            var cleanedWord = ReplaceUmlauts(match.Value);

            if (!string.IsNullOrEmpty(cleanedWord))
            {
                tokens.Add(new Token(cleanedWord, match.Index, match.Index + cleanedWord.Length));
            }
        }

        return tokens;
    }

    private string ReplaceUmlauts(string word)
    {
        if (!_config.ReplaceUmlaut) return word;

        return word.Replace("ä", "ae")
            .Replace("ö", "oe")
            .Replace("ü", "ue")
            .Replace("ß", "ss")
            .Replace("Ä", "AE")
            .Replace("Ö", "OE")
            .Replace("Ü", "UE");
    }
}