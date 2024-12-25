using SimilarityTextComparison.Domain.Models.TextProcessing;

namespace SimilarityTextComparison.Domain.Interfaces;

public interface IForwardReferenceManager
{
    /// <summary>
    /// Crea un dizionario di riferimenti avanzati (forward references) per ottimizzare il matching.
    /// Per ogni sequenza di token ripetuta nel testo, viene creata una forward reference che punta
    /// alla successiva occorrenza della stessa sequenza. Questo permette di saltare rapidamente alle
    /// ripetizioni e migliorare l'efficienza del matching.
    ///
    /// **Esempio di forward reference:**
    /// - Se la sequenza "cat sat" appare alla posizione 1 e si ripete alla posizione 6,
    ///   viene creato un riferimento: 1 → 6.
    /// </summary>
    /// <param name="text">Il testo da analizzare.</param>
    /// <returns>Un dizionario che mappa la posizione iniziale della sequenza alla posizione successiva.</returns>
    Dictionary<int, int> CreateForwardReferences(ProcessedText text);
}