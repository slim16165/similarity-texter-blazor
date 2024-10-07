using ChatGPT_Splitter_Blazor_New.TextComparer.Model.Comparison;
using ChatGPT_Splitter_Blazor_New.TextComparer.Model.TextProcessing;

namespace ChatGPT_Splitter_Blazor_New.TextComparer.Services.Interfaces;

public interface ISimTexter
{
    Task<List<List<MatchSegment>>> CompareAsync(List<MyInputText> inputTexts);
}