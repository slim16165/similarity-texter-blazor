# ![Blazor](https://img.shields.io/badge/Blazor-5.0-blue.svg) ![.NET](https://img.shields.io/badge/.NET-6.0+-purple.svg) ![License](https://img.shields.io/badge/License-CC%20BY--NC--SA%204.0-green.svg) ![Alpha](https://img.shields.io/badge/Release-Alpha-orange.svg)

## Descrizione

**Similarity Texter Blazor** è un'applicazione web moderna sviluppata con Blazor, progettata per confrontare due testi e identificare segmenti simili. Si basa sull'algoritmo "sim_text" di **Dick Grune** e incorpora le idee del progetto originale **"Similarity Texter"** di **Sofia Kalaidopoulou**. Il codice JS originale è disponibile nella cartella `\OriginalSimtexterAppJs`.

## Stato del Progetto

🚧 **Alpha Release**: In fase di sviluppo attivo. Alcune funzionalità potrebbero essere incomplete o soggette a modifiche.

## Caratteristiche

- **Confronto Testuale Avanzato**: Utilizza l'algoritmo "sim_text" per identificare segmenti simili.
- **Interfaccia Intuitiva**: Sviluppata con Blazor per un'esperienza utente fluida e moderna.
- **Opzioni Configurabili**: Personalizza il confronto ignorando maiuscole/minuscole, numeri, punteggiatura e sostituendo gli umlaut.
- **Ampia Gamma di Input**: Supporta il caricamento di file `.txt` e l'inserimento manuale di testo.
- **Risultati Visivi**: Segmenti simili evidenziati direttamente nei testi caricati.
- **Esportazione e Stampa**: Genera riepiloghi e stampa i risultati del confronto.

## Tecnologie Utilizzate

![Blazor](https://img.shields.io/badge/Blazor-5.0-blue.svg) ![.NET](https://img.shields.io/badge/.NET-6.0+-purple.svg) ![xUnit](https://img.shields.io/badge/xUnit-2.4.1-blue.svg) ![Moq](https://img.shields.io/badge/Moq-4.16.1-green.svg)

- **Blazor**: Framework per la creazione di interfacce utente interattive.
- **.NET 6+**: Piattaforma di sviluppo per applicazioni web robuste.
- **Moq & xUnit**: Strumenti di testing per garantire qualità e affidabilità del codice.

## Struttura del Progetto

```
SimilarityTexterBlazor/
├── Application/
│   ├── TextComparer.cs
│   └── MatchingPipeline.cs
├── Domain/
│   ├── MatchSegment.cs
│   └── ForwardReferenceManager.cs
├── Infrastructure/
│   ├── TextComparisonConfiguration.cs
│   └── StorageService.cs
├── Blazor/
│   ├── ComparisonDashboard.razor
│   └── Settings.razor
└── OriginalSimtexterAppJs/
    └── app.js
```

## Utilizzo

1. **Carica o Inserisci i Testi**
   - Carica file `.txt` tramite il pannello di upload.
   - Oppure inserisci manualmente il testo nei campi dedicati.

2. **Configura le Impostazioni**
   - Personalizza le opzioni di confronto tramite il pannello "Impostazioni".

3. **Esegui il Confronto**
   - Premi "Confronta" per avviare l'elaborazione.

4. **Visualizza i Risultati**
   - I segmenti simili saranno evidenziati nei testi.
   - Esporta o stampa i risultati per analisi successive.

## Licenza

Questo progetto è distribuito sotto la licenza [CC BY-NC-SA 4.0](LICENSE), in conformità con:

- **Dick Grune**: L'algoritmo "sim_text" è distribuito sotto licenza [BSD-3-Clause](https://opensource.org/licenses/BSD-3-Clause).
- **Sofia Kalaidopoulou**: Il progetto "Similarity Texter" è rilasciato sotto licenza [CC BY-NC-SA 4.0](https://creativecommons.org/licenses/by-nc-sa/4.0/).

I contributi di entrambi gli autori sono riconosciuti esplicitamente nel presente README.

## Riconoscimenti

- **Dick Grune**: Creatore dell'algoritmo "sim_text". Maggiori informazioni su [dickgrune.com](https://dickgrune.com/Programs/similarity_text/).
- **Sofia Kalaidopoulou**: Autrice del progetto originale **Similarity Texter**. Visita [Similarity Texter](https://people.f4.htw-berlin.de/~weberwu/simtexter/app.html) per ulteriori dettagli.

## Citazioni

- **Dick Grune**: Grune, D. (1989). *sim_text: A text similarity tester*. Vrije Universiteit Amsterdam. Disponibile su [dickgrune.com](https://dickgrune.com/Programs/similarity_text/).
- **Sofia Kalaidopoulou**: Kalaidopoulou, S. (2016). *Similarity Texter: A text-comparison web tool based on the sim_text algorithm*. Tesi di laurea, HTW Berlin. Disponibile su [htw-berlin.de](https://people.f4.htw-berlin.de/~weberwu/simtexter/app.html).

## Miglioramenti Futuri

- 📸 **Screenshot e Demo Video**: Aggiungi elementi visivi per migliorare la comprensione del progetto.
- ❓ **Sezione FAQ**: Risposte a domande comuni degli utenti.
- 📜 **Dettagli sulle Dipendenze**: Lista dettagliata delle dipendenze del progetto.

## Supporto

Hai bisogno di aiuto o hai un suggerimento? [Crea un'issue su GitHub](https://github.com/slim16165/similarity-texter-blazor/issues) oppure contattami tramite [LinkedIn](https://www.linkedin.com/in/gianluigisalvi/) o [GitHub](https://github.com/slim16165/similarity-texter-blazor).