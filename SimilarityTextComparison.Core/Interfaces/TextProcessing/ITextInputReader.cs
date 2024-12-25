namespace SimilarityTextComparison.Domain.Interfaces.TextProcessing;

public interface ITextInputReader
{
    /// <summary>
    /// Legge l'input di testo HTML e restituisce il contenuto come stringa pulita.
    /// </summary>
    /// <param name="text">L'input HTML da cui estrarre il testo.</param>
    /// <returns>Una task che rappresenta il testo estratto e pulito.</returns>
    Task<string> ReadTextInputAsync(string htmlInput);

    string CleanHtmlInput(string htmlInput);
}