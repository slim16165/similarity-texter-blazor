using ChatGPT_Splitter_Blazor_New.Pages.TextComparer.Model.TextProcessing;
using ChatGPT_Splitter_Blazor_New.TextComparer.Model.Comparison;

namespace ChatGPT_Splitter_Blazor_New.TextComparer.Services.Interfaces;

public interface ISimTexter
{
    Task<List<List<MatchSegment>>> CompareAsync(List<MyInputText> inputTexts);
}