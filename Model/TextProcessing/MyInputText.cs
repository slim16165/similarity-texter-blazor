using Microsoft.AspNetCore.Components.Forms;

namespace ChatGPT_Splitter_Blazor_New.Pages.TextComparer.Model.TextProcessing;
public class MyInputText
{
    public string TabPaneId { get; private set; }
    public string Mode { get; private set; }
    public bool IsHTML { get; private set; }
    public string FileName { get; private set; }
    public string Text { get; private set; }
    public string InputMode { get; set; }
    public int NrOfCharacters { get; set; }
    public int NrOfWords { get; set; }

    public MyInputText(string mode, IBrowserFile file = null, string text = null, string tabPaneId = null)
    {
        TabPaneId = tabPaneId;
        Mode = mode;
        IsHTML = false;
        FileName = file?.Name;
        Text = text;
    }

    /// <summary>
    /// Returns the approximate number of pages of the input string.
    /// </summary>
    /// <param name="maxCharactersPerPage">The maximum number of characters per page.</param>
    /// <returns>The approximate number of pages.</returns>
    public int GetNumberOfPages(int maxCharactersPerPage)
    {
        return Text?.Length / maxCharactersPerPage ?? 0;
    }

    /// <summary>
    /// Resets the fields of MyInputText.
    /// </summary>
    public void Reset()
    {
        TabPaneId = null;
        Mode = null;
        FileName = null;
        Text = null;
    }

    /// <summary>
    /// Sets the fields for the file input.
    /// </summary>
    /// <param name="file">The file selected by the user.</param>
    /// <param name="text">The file input string.</param>
    /// <param name="tabPaneId">The ID of the tab pane.</param>
    public void SetFileInput(IBrowserFile file, string text, string tabPaneId)
    {
        TabPaneId = tabPaneId;
        Mode = "File";
        FileName = file.Name;
        Text = text;
    }

    /// <summary>
    /// Sets the fields for the text input.
    /// </summary>
    /// <param name="text">The text input string.</param>
    /// <param name="tabPaneId">The ID of the tab pane.</param>
    public void SetTextInput(string text, string tabPaneId)
    {
        TabPaneId = tabPaneId;
        Mode = "Text";
        FileName = IsHTML ? "HTML text input" : "Plain text input";
        Text = text;
    }

    /// <summary>
    /// Sets the HTML option.
    /// </summary>
    /// <param name="newValue">Whether the input is HTML.</param>
    public void SetHTMLOption(bool newValue)
    {
        IsHTML = newValue;
    }
}