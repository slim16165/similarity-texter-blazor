namespace ChatGPT_Splitter_Blazor_New.TextComparer.Model.TextProcessing;

/// <summary>
/// Rappresenta un token del testo, con posizione nel testo.
/// </summary>
public class Token
{
    /// <summary>
    /// Il testo della parola dopo essere stato "pulito" secondo le opzioni di confronto.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// L'indice del primo carattere della parola nell'input originale (inclusivo).
    /// </summary>
    public int TextBeginPos { get; }

    /// <summary>
    /// L'indice dell'ultimo carattere della parola nell'input originale (non incluso).
    /// </summary>
    public int TextEndPos { get; }

    /// <summary>
    /// Costruttore della classe Token.
    /// </summary>
    /// <param name="text">Il testo della parola dopo essere stato pulito.</param>
    /// <param name="textBeginPos">L'indice del primo carattere della parola.</param>
    /// <param name="textEndPos">L'indice dell'ultimo carattere della parola (non incluso).</param>
    public Token(string text = "", int textBeginPos = 0, int textEndPos = 0)
    {
        Text = text;
        TextBeginPos = textBeginPos;
        TextEndPos = textEndPos;
    }
}
