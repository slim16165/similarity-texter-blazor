namespace SimilarityTextComparison.Core.Models.Position;

public class CharacterPosition : PositionalEntity
{

    /// <summary>
    /// L'indice del primo carattere della parola nell'input originale (inclusivo).
    /// </summary>
    public int TextBeginPos => base.BeginPosition;

    /// <summary>
    /// L'indice dell'ultimo carattere della parola nell'input originale (non incluso).
    /// </summary>
    public int TextEndPos => base.EndPosition;

    public CharacterPosition(int beginCharPos, int endCharPos)
        : base(beginCharPos, endCharPos, PositionUnit.Character)
    {
    }

    public bool Contains(int charPosition)
    {
        return charPosition >= BeginPosition && charPosition < EndPosition;
    }
}