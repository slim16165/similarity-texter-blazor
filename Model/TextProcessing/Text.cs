namespace ChatGPT_Splitter_Blazor_New.Pages.TextComparer.Model.TextProcessing;


public class Text
{
    public string InputMode { get; private set; }
    public string FileName { get; private set; }
    public int TkBeginPos { get; private set; }
    public int TkEndPos { get; private set; }
    public int NrOfCharacters { get; private set; }
    public int NrOfWords { get; private set; }

    // Costruttore che inizializza le proprietà
    public Text(string inputMode, int nrOfCharacters, int nrOfWords, string fileName, int tkBeginPos = 0, int tkEndPos = 0)
    {
        InputMode = inputMode;
        FileName = fileName;
        TkBeginPos = tkBeginPos;
        TkEndPos = tkEndPos;
        NrOfCharacters = nrOfCharacters;
        NrOfWords = nrOfWords;
    }
}
