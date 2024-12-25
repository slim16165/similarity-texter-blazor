namespace SimilarityTextComparison.Domain.Models.TextProcessing;

public class InputInfo
{
    public string InputMode { get; }
    public string FileName { get; }
    public string Text { get; set; }

    public InputInfo(string inputMode, string fileName, string text)
    {
        InputMode = inputMode;
        FileName = fileName;
        Text = text;
    }
}