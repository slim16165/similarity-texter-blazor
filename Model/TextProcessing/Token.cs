namespace ChatGPT_Splitter_Blazor_New.Pages.TextComparer.Model.TextProcessing;

/// <summary>
/// Rappresenta un token del testo, con posizione nel testo.
/// </summary>
public class Token
{
    /// <summary>
    /// Il testo della parola dopo essere stato "pulito" secondo le opzioni di confronto.
    /// </summary>
    public string Text { get; private set; }

    /// <summary>
    /// L'indice del primo carattere della parola nell'input originale (inclusivo).
    /// </summary>
    public int TxtBeginPos { get; private set; }

    /// <summary>
    /// L'indice dell'ultimo carattere della parola nell'input originale (non incluso).
    /// </summary>
    public int TxtEndPos { get; private set; }

    /// <summary>
    /// Costruttore della classe Token.
    /// </summary>
    /// <param name="text">Il testo della parola dopo essere stato pulito.</param>
    /// <param name="txtBeginPos">L'indice del primo carattere della parola.</param>
    /// <param name="txtEndPos">L'indice dell'ultimo carattere della parola (non incluso).</param>
    public Token(string text = "", int txtBeginPos = 0, int txtEndPos = 0)
    {
        Text = text;
        TxtBeginPos = txtBeginPos;
        TxtEndPos = txtEndPos;
    }
}
