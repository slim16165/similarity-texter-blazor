using ChatGPT_Splitter_Blazor_New.Pages.TextComparer.Services;
using ChatGPT_Splitter_Blazor_New.TextComparer.Components;
using Microsoft.AspNetCore.Components;

namespace ChatGPT_Splitter_Blazor_New.TextComparer.Views;

// TextComparer.razor.cs
public partial class TextComparer : ComponentBase
{
    private string MyInputText1 { get; set; }
    private string MyInputText2 { get; set; }

    private bool isLoading = false;
    private string alertMessage;
    private string alertType;
    private List<ComparisonResult.ComparisonResultItem> comparisonResults;

    // Metodo per confrontare i testi
    private async Task CompareTexts()
    {
        if (string.IsNullOrWhiteSpace(MyInputText1) || string.IsNullOrWhiteSpace(MyInputText2))
        {
            alertMessage = "Entrambi i testi devono essere compilati.";
            alertType = "warning";
            return;
        }

        isLoading = true;
        alertMessage = string.Empty;
        comparisonResults = null;
        StateHasChanged();

        try
        {
            var result = await ControllerService.Instance.CompareTextsAsync(MyInputText1, MyInputText2);
            comparisonResults = new List<ComparisonResult.ComparisonResultItem>
            {
                new ComparisonResult.ComparisonResultItem { Index = 1, Description = result }
            };
            alertMessage = "Confronto completato.";
            alertType = "info";
        }
        catch (Exception ex)
        {
            alertMessage = $"Errore durante il confronto: {ex.Message}";
            alertType = "danger";
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }
}