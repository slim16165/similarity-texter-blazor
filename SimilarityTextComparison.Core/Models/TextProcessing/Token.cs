using SimilarityTextComparison.Core.Models.Position;

namespace SimilarityTextComparison.Core.Models.TextProcessing;

public class Token : CharacterPosition
{
    /// <summary>
    /// Il testo della parola dopo essere stato "pulito" secondo le opzioni di confronto.
    /// </summary>
    public string Text { get; }

    

    /// <summary>
    /// Costruttore della classe Token.
    /// Ora calcola la posizione finale basata sulla lunghezza del testo.
    /// </summary>
    /// <param name="text">Il testo della parola dopo essere stato pulito.</param>
    /// <param name="textBeginPos">L'indice del primo carattere della parola nell'input originale.</param>
    public Token(string text, int textBeginPos)
        : base(textBeginPos, textBeginPos + text.Length)
    {
        Text = text;
    }
}