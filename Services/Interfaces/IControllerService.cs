namespace ChatGPT_Splitter_Blazor_New.TextComparer.Services.Interfaces;

public interface IControllerService
{
    Task<string> CompareTextsAsync(string input1, string input2);
    // Aggiungi altri metodi necessari
}