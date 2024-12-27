using System.Text.RegularExpressions;
using SimilarityTextComparison.Domain.Interfaces.TextProcessing;
using SimilarityTextComparison.Infrastructure.Services;

namespace SimilarityTextComparison.Domain.Services.TextPreProcessing;

/// <summary>
/// Classe responsabile della pulizia del testo in base a una configurazione.
/// Le opzioni di configurazione vengono tradotte in una serie di regex per rimuovere
/// determinati caratteri (numeri, punteggiatura) e per applicare normalizzazioni (ad esempio, minuscolo).
/// </summary>
public class TextProcessor : ITextProcessor
{
    private readonly TextComparisonConfiguration _config;
    private readonly Regex _filterRegex;

    /// <summary>
    /// Costruttore della classe TextProcessor.
    /// Inizializza la configurazione e genera una regex basata sulle opzioni specificate nella configurazione.
    /// </summary>
    /// <param name="config">La configurazione che definisce le opzioni di pulizia del testo.</param>
    public TextProcessor(TextComparisonConfiguration config)
    {
        _config = config;
        // Trasforma le impostazioni di configurazione in una regex per filtrare il testo
        _filterRegex = BuildCleanerRegex();
    }

    /// <summary>
    /// Pulisce il testo applicando i filtri definiti dalla configurazione.
    /// Le opzioni di configurazione controllano quali caratteri rimuovere (numeri, punteggiatura)
    /// e se il testo deve essere convertito in minuscolo.
    /// </summary>
    /// <param name="text">Il testo da pulire.</param>
    /// <returns>Il testo pulito e normalizzato.</returns>
    public string CleanText(string text)
    {
        // 1) Applica i filtri regex (rimuove numeri, punteggiatura -> sostituisce con spazio)
        text = ApplyRegexFilters(text);

        // 2) (Opzionale) Collassa subito gli spazi, così eviti di portarti avanti un testo pieno di spazi
        text = CollapseSpaces(text);

        // 3) Normalizza il case (minuscolo) se necessario
        text = ApplyCaseNormalization(text);

        // 4) Sostituzione degli umlaut (se config attivo)
        text = ReplaceUmlautsIfNeeded(text);

        // 5) Ricollassa spazi (se gli umlaut generano ulteriori spazi, in genere no, ma per sicurezza)
        text = CollapseSpaces(text);

        return text;
    }

    /// <summary>
    /// Applica una regex precompilata per sostituire caratteri specifici (numeri, punteggiatura) con uno spazio.
    /// La regex è stata costruita in base alla configurazione fornita.
    /// </summary>
    /// <param name="text">Il testo a cui applicare i filtri regex.</param>
    /// <returns>Il testo filtrato.</returns>
    private string ApplyRegexFilters(string text)
    {
        // Se la regex è stata generata, applicala, altrimenti restituisci il testo non modificato
        return _filterRegex != null ? _filterRegex.Replace(text, " ") : text;
    }

    /// <summary>
    /// Normalizza il case del testo in minuscolo se la configurazione richiede di ignorare la differenza tra maiuscole e minuscole.
    /// </summary>
    /// <param name="text">Il testo da normalizzare.</param>
    /// <returns>Il testo convertito in minuscolo o originale se non è richiesta la normalizzazione.</returns>
    private string ApplyCaseNormalization(string text)
    {
        // Controlla se la configurazione richiede di ignorare le maiuscole/minuscole
        return _config.IgnoreLetterCase ? text.ToLowerInvariant() : text;
    }

    /// <summary>
    /// Costruisce una regex basata sulle impostazioni di configurazione.
    /// La regex combina più pattern per rimuovere numeri e punteggiatura, se configurato.
    /// </summary>
    /// <returns>
    /// Una regex che filtra i caratteri definiti dalla configurazione (numeri, punteggiatura),
    /// oppure null se non è necessario alcun filtro.
    /// </returns>
    private Regex? BuildCleanerRegex()
    {
        var patterns = new List<string>();

        // Aggiungi il pattern per ignorare i numeri, se configurato
        if (_config.IgnoreNumbers)
        {
            patterns.Add(@"\p{N}"); // Pattern per i numeri
        }

        // Aggiungi il pattern per ignorare la punteggiatura, se configurato
        if (_config.IgnorePunctuation)
        {
            patterns.Add(@"\p{P}"); // Pattern per la punteggiatura
        }

        if (_config.IgnorePunctuation)
        {
            // Aggiungi la classe \p{P} (punteggiatura) + \p{S} (simboli),
            // se vuoi davvero eguagliare un certo comportamento "ignorare punteggiatura" in modo più esteso
            patterns.Add(@"\p{P}\p{S}");
        }

        // Se sono stati definiti dei pattern, creiamo una regex combinata
        var combinedPattern = string.Join("", patterns);

        // Restituisce una regex compilata che filtra numeri e/o punteggiatura, o null se non ci sono filtri
        return !string.IsNullOrEmpty(combinedPattern) ? new Regex($"[{combinedPattern}]", RegexOptions.Compiled) : null;
    }



    private string CollapseSpaces(string text)
    {
        // Sostituisce una o più occorrenze di spazio bianco con un singolo spazio
        text = Regex.Replace(text, @"\s+", " ");
        // Taglia spazi iniziali e finali
        return text.Trim();
    }


    private string ReplaceUmlautsIfNeeded(string text)
    {
        if (!_config.ReplaceUmlaut)
            return text;

        // Esempio di sostituzione base:
        return text
            .Replace("ä", "ae")
            .Replace("ö", "oe")
            .Replace("ü", "ue")
            .Replace("ß", "ss")
            .Replace("Ä", "AE")
            .Replace("Ö", "OE")
            .Replace("Ü", "UE");
    }

}
