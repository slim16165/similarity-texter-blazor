namespace SimilarityTextComparison.Domain.Models.Matching;

public class ForwardReference
{
    public int From { get; set; }
    public int To { get; set; }

    public ForwardReference(int from, int to)
    {
        From = from;
        To = to;
    }
}