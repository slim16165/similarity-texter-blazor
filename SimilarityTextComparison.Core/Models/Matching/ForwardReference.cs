namespace SimilarityTextComparison.Domain.Models.Matching;

public class ForwardReference
{
    public int FromTokenPos { get; set; }
    public int ToTokenPos { get; set; }

    // Sequenza di token collegata
    public string Sequence { get; set; }

    public ForwardReference(int from, int to, string sequence)
    {
        FromTokenPos = from;
        ToTokenPos = to;
        Sequence = sequence;
    }

    // Override del metodo ToString() per una rappresentazione più chiara
    public override string ToString()
    {
        return $"ForwardReference: '{Sequence}' from position {FromTokenPos} to position {ToTokenPos}";
    }
}