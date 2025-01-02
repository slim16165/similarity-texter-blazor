# Similarity Texter Blazor (alpha release)

## Descrizione

**Similarity Texter Blazor** è un'applicazione web moderna sviluppata con Blazor, progettata per confrontare due testi e identificare segmenti simili. La logica principale del confronto si basa sull'algoritmo "sim_text" sviluppato da **Dick Grune**, riconosciuto per il suo lavoro pionieristico nel rilevamento delle somiglianze testuali.

L'applicazione incorpora inoltre le idee del progetto originale **"similarity texter"** creato da **Sofia Kalaidopoulou**, che ha fornito l'ispirazione per la struttura e le funzionalità di base.

## Stato del Progetto

Questa versione è attualmente un'alpha, ancora in fase di sviluppo attivo. Alcune funzionalità potrebbero essere incomplete o soggette a cambiamenti significativi.

## Caratteristiche Principali

- **Confronto Testuale Avanzato**: Identifica segmenti simili utilizzando l'algoritmo "sim_text".
- **Interfaccia Utente Intuitiva**: Sviluppata con Blazor per garantire un'esperienza utente fluida e moderna.
- **Opzioni Configurabili**: Personalizza il confronto ignorando maiuscole/minuscole, numeri, punteggiatura e sostituendo gli umlaut.
- **Ampia Gamma di Input**: Supporto per caricamento file .txt e inserimento manuale di testo.
- **Risultati Visivi**: Segmenti simili evidenziati direttamente nei testi caricati.
- **Esportazione e Stampa**: Possibilità di generare riepiloghi e stampare i risultati del confronto.

## Tecnologie Utilizzate

- **Blazor**: Framework per la creazione di interfacce utente interattive.
- **.NET 6+**: Piattaforma di sviluppo per applicazioni web robuste.
- **Moq & xUnit**: Strumenti di testing per garantire qualità e affidabilità del codice.

## Struttura del Progetto

- **Application**: Logica principale dell'applicazione.
  - `TextComparer.cs`: Gestisce il confronto testuale.
  - `MatchingPipeline.cs`: Pipeline per il rilevamento delle somiglianze.
- **Domain**: Modelli e interfacce principali.
  - `MatchSegment.cs`: Rappresenta i segmenti simili trovati.
  - `ForwardReferenceManager.cs`: Gestione dei riferimenti avanti nell'algoritmo.
- **Infrastructure**: Implementazioni concrete dei servizi richiesti.
  - `TextComparisonConfiguration.cs`: Configurazione delle opzioni di confronto.
  - `StorageService.cs`: Gestione dello storage locale.
- **Blazor**: Componenti dell'interfaccia utente.
  - `ComparisonDashboard.razor`: Visualizzazione dei risultati del confronto.
  - `Settings.razor`: Configurazione delle opzioni dell'applicazione.

## Installazione

### Prerequisiti

- **.NET 6 SDK o superiore**: [Scarica .NET](https://dotnet.microsoft.com/download)
- **Visual Studio 2022 o Visual Studio Code**: IDE consigliati per lo sviluppo.

### Passaggi

1. **Clona il Repository**

   ```bash
   git clone https://github.com/tuo-username/similarity-texter-blazor.git
   cd similarity-texter-blazor
   ```

2. **Ripristina i Pacchetti NuGet**

   ```bash
   dotnet restore
   ```

3. **Compila il Progetto**

   ```bash
   dotnet build
   ```

4. **Esegui l'Applicazione**

   ```bash
   dotnet run --project SimilarityTextComparison.Blazor
   ```

5. **Accedi all'Applicazione**

   Apri il tuo browser e naviga all'indirizzo [https://localhost:5001](https://localhost:5001).

## Utilizzo

1. **Carica o Inserisci i Testi**
   - Carica file di testo tramite il pannello di upload.
   - Oppure inserisci manualmente il testo nei campi dedicati.

2. **Configura le Impostazioni**
   - Personalizza le opzioni di confronto tramite il pannello "Impostazioni".

3. **Esegui il Confronto**
   - Premi "Confronta" per avviare l'elaborazione.

4. **Visualizza i Risultati**
   - I segmenti simili saranno evidenziati nei testi confrontati.
   - Esporta o stampa i risultati per analisi successive.

## Contribuire

Siamo aperti ai contributi! Segui questi passaggi per partecipare al progetto:

1. **Fork del Repository**
2. **Crea un Branch per le Tue Modifiche**

   ```bash
   git checkout -b feature/nome-feature
   ```

3. **Effettua le Modifiche e Commit**

   ```bash
   git commit -m "Descrizione delle modifiche"
   ```

4. **Pusha il Branch**

   ```bash
   git push origin feature/nome-feature
   ```

5. **Crea una Pull Request**

## Licenza

Questo progetto è distribuito sotto la licenza [MIT](LICENSE). Questa licenza è compatibile con le opere di **Dick Grune** e **Sofia Kalaidopoulou**, i cui contributi sono riconosciuti esplicitamente nel presente README.

## Riconoscimenti

- **Dick Grune**: Autore dell'algoritmo "sim_text". Per ulteriori informazioni, visita il suo [sito web](https://dickgrune.com/Programs/similarity_text/).
- **Sofia Kalaidopoulou**: Creatrice del progetto originale "similarity texter". Puoi visitare il suo [profilo LinkedIn](https://linkedin.com/in/sofia-kalaidopoulou).

Entrambi i loro contributi sono stati fondamentali per lo sviluppo di questa applicazione.

## Suggerimenti per Miglioramenti Futuri

- **Screenshot e Demo Video**: Aggiungi elementi visivi per migliorare la comprensione del progetto.
- **Sezione FAQ**: Risposte a domande comuni degli utenti.
- **Dettagli sulle Dipendenze**: Lista dettagliata delle dipendenze del progetto.
- **Documentazione Avanzata**: Guide per sviluppatori e amministratori di sistema.

## Supporto

Hai bisogno di aiuto o hai un suggerimento? Non esitare a creare un'issue su GitHub o a contattarmi tramite [LinkedIn](https://www.linkedin.com/in/gianluigisalvi/) o [GitHub](https://github.com/slim16165/similarity-texter-blazor).

