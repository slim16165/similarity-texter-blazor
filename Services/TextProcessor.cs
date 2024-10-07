using System.Text.RegularExpressions;

namespace ChatGPT_Splitter_Blazor_New.TextComparer.Services;

public class TextProcessor
{
    private readonly Configuration _config;
    private readonly Regex _filterRegex;

    public TextProcessor(Configuration config)
    {
        _config = config;
        _filterRegex = BuildFilterRegex();
    }

    public string CleanText(string text)
    {
        text = ApplyRegexFilters(text);
        text = ApplyCaseNormalization(text);
        return text;
    }

    private string ApplyRegexFilters(string text)
    {
        return _filterRegex != null ? _filterRegex.Replace(text, " ") : text;
    }

    private string ApplyCaseNormalization(string text)
    {
        return _config.IgnoreLetterCase ? text.ToLowerInvariant() : text;
    }


    private Regex? BuildFilterRegex()
    {
        var patterns = new List<string>();

        if (_config.IgnoreNumbers) patterns.Add(@"\p{N}");
        if (_config.IgnorePunctuation) patterns.Add(@"\p{P}");

        var combinedPattern = string.Join("", patterns);
        return !string.IsNullOrEmpty(combinedPattern) ? new Regex($"[{combinedPattern}]", RegexOptions.Compiled) : null;
    }

}