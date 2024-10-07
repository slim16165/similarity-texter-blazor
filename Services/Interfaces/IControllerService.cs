using ChatGPT_Splitter_Blazor_New.TextComparer.Model.Comparison;

namespace ChatGPT_Splitter_Blazor_New.TextComparer.Services.Interfaces;

public interface IControllerService
{
    Task<List<List<MatchSegment>>> CompareTextsAsync(string input1, string input2);
}