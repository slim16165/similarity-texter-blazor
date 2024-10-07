namespace ChatGPT_Splitter_Blazor_New.Pages.TextComparer.Model.TextProcessing;


public class MyText
{
    public string InputMode { get; private set; }
    public string FileName { get; private set; }
    public int TokenkBeginPos { get; private set; }
    public int TokenEndPos { get; set; }
    public int NrOfCharacters { get; private set; }
    public int NrOfWords { get; private set; }

    // Costruttore che inizializza le proprietà
    public MyText(string inputMode, int numberOfCharacters, int numberOfWords, string fileName, int tokenBeginPosition = 0, int tokenEndPosition = 0)
    {
        InputMode = inputMode;
        FileName = fileName;
        TokenkBeginPos = tokenBeginPosition;
        TokenEndPos = tokenEndPosition;
        NrOfCharacters = numberOfCharacters;
        NrOfWords = numberOfWords;
    }
}
