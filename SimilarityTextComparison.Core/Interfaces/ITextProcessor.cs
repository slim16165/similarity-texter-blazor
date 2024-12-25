namespace SimilarityTextComparison.Domain.Interfaces;

public interface ITextProcessor
{
    /// <summary>
    /// Pulisce il testo applicando i filtri definiti dalla configurazione.
    /// Le opzioni di configurazione controllano quali caratteri rimuovere (numeri, punteggiatura)
    /// e se il testo deve essere convertito in minuscolo.
    /// </summary>
    /// <param name="text">Il testo da pulire.</param>
    /// <returns>Il testo pulito e normalizzato.</returns>
    string CleanText(string text);
}