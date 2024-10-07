using System.Text.RegularExpressions;
using ChatGPT_Splitter_Blazor_New.TextComparer.Model.Comparison;
using ChatGPT_Splitter_Blazor_New.TextComparer.Model.TextProcessing;
using ChatGPT_Splitter_Blazor_New.TextComparer.Services.Interfaces;

namespace ChatGPT_Splitter_Blazor_New.TextComparer.Services;

public class SimTexter : ISimTexter
{
    private readonly Configuration _config;
    private readonly TextProcessor _textProcessor;
    private readonly Tokenizer _tokenizer;
    private readonly ForwardReferenceManager _forwardRefMgr;
    private readonly Matcher _matcher;
    private readonly StyleApplier _styleApplier;

    public SimTexter(IStorageService storageService)
    {
        _config = new Configuration(storageService ?? throw new ArgumentNullException(nameof(storageService)));
        _textProcessor = new TextProcessor(_config);
        _tokenizer = new Tokenizer(_config);
        _forwardRefMgr = new ForwardReferenceManager(_config);
        _matcher = new Matcher(_config);
        _styleApplier = new StyleApplier();
    }

    public async Task<List<List<MatchSegment>>> CompareAsync(List<MyInputText> inputTexts)
    {
        if (inputTexts == null || inputTexts.Count < 2)
            throw new ArgumentException("Sono necessari almeno due testi per il confronto.");

        // Processa i testi di input e ottieni testi e token
        var (texts, tokens) = ProcessInputTexts(inputTexts);

        // Crea i riferimenti avanti per il primo testo
        var forwardReferences = _forwardRefMgr.CreateForwardReferences(texts[0], tokens);

        // Trova le similarità tra il primo e il secondo testo
        var similarities = _matcher.FindMatches(0, 1, texts[0], texts[1], forwardReferences, tokens);

        if (similarities.Count > 0)
        {
            return _styleApplier.ApplyStyles(similarities);
        }
        else
        {
            throw new Exception("Nessuna similarità trovata.");
        }
    }

    private (List<MyText> texts, List<Token> tokens) ProcessInputTexts(List<MyInputText> inputTexts)
    {
        var texts = new List<MyText>();
        var tokens = new List<Token>();
        int currentTokenPosition = 0;

        foreach (var inputText in inputTexts)
        {
            var (text, textTokens) = ProcessSingleInputText(inputText, currentTokenPosition);

            texts.Add(text);
            tokens.AddRange(textTokens);
            currentTokenPosition += textTokens.Count;
        }

        return (texts, tokens);
    }

    private (MyText text, List<Token> tokens) ProcessSingleInputText(MyInputText inputText, int startingTokenPosition)
    {
        string cleanText = _textProcessor.CleanText(inputText.Text);
        var tokens = _tokenizer.Tokenize(cleanText);

        int wordCount = CountWords(cleanText);

        var text = new MyText(
            inputMode: inputText.Mode,
            numberOfCharacters: inputText.Text.Length,
            numberOfWords: wordCount,
            fileName: inputText.FileName,
            tokenBeginPosition: startingTokenPosition,
            tokenEndPosition: startingTokenPosition + tokens.Count);

        return (text, tokens);
    }

    private static int CountWords(string text)
    {
        return Regex.Matches(text, @"\S+").Count;
    }
}