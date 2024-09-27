namespace ChatGPT_Splitter_Blazor_New.Pages.TextComparer.Model.Comparison;

public class ScrollPosition
{
    public int TopPadding { get; }
    public int BottomPadding { get; }
    public int YPosition { get; }

    public ScrollPosition(int topPadding, int bottomPadding, int yPosition)
    {
        TopPadding = topPadding;
        BottomPadding = bottomPadding;
        YPosition = yPosition;
    }
}