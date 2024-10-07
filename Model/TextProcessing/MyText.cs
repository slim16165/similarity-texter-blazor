namespace ChatGPT_Splitter_Blazor_New.TextComparer.Model.TextProcessing;

public class MyText : PositionalEntity
{
    public string InputMode { get; private set; }
    public string FileName { get; private set; }
    public int TokenBeginPos => base.BeginPosition;
    public int TokenEndPos => base.EndPosition;
    public int NrOfCharacters { get; private set; }
    public int NrOfWords { get; private set; }

    // Costruttore che inizializza le proprietà
    public MyText(string inputMode, int numberOfCharacters, int numberOfWords, string fileName, int tokenBeginPosition, int tokenEndPosition) : base(tokenBeginPosition, tokenEndPosition, PositionUnit.Token)
    {
        InputMode = inputMode;
        FileName = fileName;
        NrOfCharacters = numberOfCharacters;
        NrOfWords = numberOfWords;
    }
}
