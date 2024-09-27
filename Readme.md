# TextComparer Blazor Application

Questo progetto è un'applicazione Blazor per il confronto di testi, porting di un'applicazione JavaScript originaria basata su Node.js e interfacce web, utilizzando le potenzialità di Blazor per il frontend. L'applicazione consente di confrontare due testi o file e mostrare le somiglianze tra di essi, fornendo un'interfaccia user-friendly per la gestione e la visualizzazione dei risultati.

## Funzionalità principali

- **Confronto di testi**: L'applicazione permette di caricare testi o file e confrontarli per trovare somiglianze.
- **Opzioni di personalizzazione**: Gli utenti possono configurare varie opzioni per il confronto, come ignorare maiuscole/minuscole, numeri, punteggiatura, o sostituire caratteri speciali come gli umlaut.
- **Visualizzazione dinamica**: I risultati del confronto vengono mostrati in un'interfaccia dinamica con supporto per scrolling e highlighting.
- **Caricamento e gestione file**: L'applicazione permette di caricare file testuali e gestirli tramite un'interfaccia semplice e intuitiva.

## Struttura del progetto

Il progetto è diviso in vari componenti principali. Qui sotto trovi una descrizione dei file più rilevanti e delle loro funzionalità.

### 1. `SimTexter.cs`
Questo file contiene la logica principale per il confronto tra testi. 
- Recupera le impostazioni dell'utente dal servizio di storage (`StorageService`).
- Tokenizza i testi di input e trova le somiglianze.
- Applica eventuali stili e visualizza i risultati.
  
**Metodi principali**:
- `CompareAsync`: esegue il confronto tra i testi.
- `_tokenizeInput`: divide il testo in token per effettuare il confronto.
- `_getSimilarities`: trova le somiglianze tra i testi e gestisce i risultati.

### 2. `MyInputText.cs`
Questo file gestisce l'input dell'utente, che può essere un testo o un file. 
- Permette di caricare file o inserire testo direttamente tramite l'interfaccia.
- Converte l'input testuale in una struttura che può essere confrontata da `SimTexter`.

**Metodi principali**:
- `SetFileInput`: gestisce il caricamento di un file di input.
- `SetTextInput`: gestisce l'inserimento diretto di testo.

### 3. `View.cs`
Gestisce la visualizzazione dei risultati e l'interfaccia utente.
- Mostra gli alert, aggiorna il pannello dei risultati, e permette l'interazione tra i componenti.
- Simula eventi come `resize` o `compare`.

**Metodi principali**:
- `ShowAlertMessage`: mostra i messaggi di avviso nell'interfaccia.
- `ShowSimilarities`: visualizza le somiglianze trovate tra i testi.
- `ToggleCompareBtn`: abilita o disabilita il pulsante di confronto.

### 4. Componenti Razor

I componenti `.razor` rappresentano i vari elementi interattivi dell'interfaccia utente, tra cui caricamento dei file, visualizzazione dei risultati, gestione delle impostazioni e altro.

- **`AlertMessage.razor`**: Gestisce i messaggi di avviso che appaiono nella UI.
- **`ComparisonResult.razor`**: Mostra i risultati del confronto, con i testi confrontati evidenziati.
- **`FileUploader.razor`**: Permette il caricamento di file da confrontare.
- **`Loader.razor`**: Visualizza un'animazione durante il caricamento.
- **`OutputTitle.razor`**: Mostra i titoli dei file di input/output.
- **`PrintDialog.razor` e `PrintSummary.razor`**: Gestiscono la funzionalità di stampa dei risultati del confronto.
- **`Settings.razor`**: Permette di configurare le impostazioni di confronto (es. ignorare punteggiatura, numeri, ecc.).
- **`Statistics.razor`**: Visualizza statistiche sul confronto, come il numero di parole o caratteri nei testi.

### 5. `StorageService.cs`
Gestisce lo storage locale delle impostazioni utilizzando JavaScript Interop in Blazor per interagire con il `localStorage`.

**Metodi principali**:
- `GetItemValueByKeyAsync`: recupera un valore specifico dallo storage locale.
- `SetItemValueById`: imposta il valore di una specifica impostazione.

## Requisiti

- .NET 6.0 SDK
- Blazor WebAssembly

## Installazione

1. Clona il repository:

   ```bash
   git clone https://github.com/tuo-repository-url.git
