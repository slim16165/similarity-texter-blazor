# ![Blazor](https://img.shields.io/badge/Blazor-5.0-blue.svg) ![.NET](https://img.shields.io/badge/.NET-6.0+-purple.svg) ![License](https://img.shields.io/badge/License-CC%20BY--NC--SA%204.0-green.svg) ![Alpha](https://img.shields.io/badge/Release-Alpha-orange.svg)

# **Similarity Texter Blazor**

**Similarity Texter Blazor** è un'applicazione web moderna sviluppata con Blazor, progettata per confrontare due testi e identificare segmenti simili. Questa versione è una modernizzazione del progetto originale **Similarity Texter** di **Sofia Kalaidopoulou**, implementando l'algoritmo **sim_text** di **Dick Grune** in un'interfaccia utente intuitiva e reattiva.

---

## 📚 **Indice dei Contenuti**
1. [Descrizione](#descrizione)
2. [Stato del Progetto](#stato-del-progetto)
3. [Caratteristiche](#caratteristiche)
4. [Tecnologie Utilizzate](#tecnologie-utilizzate)
5. [Struttura del Progetto](#struttura-del-progetto)
6. [OriginalSimtexterAppJs](#originalsimtexterappjs)
7. [Utilizzo](#utilizzo)
8. [Documentazione Dettagliata](#documentazione-dettagliata)
9. [Licenza](#licenza)
10. [Riconoscimenti](#riconoscimenti)
11. [Citazioni](#citazioni)
12. [Miglioramenti Futuri](#miglioramenti-futuri)
13. [Supporto](#supporto)

---

## 📝 **Descrizione**

**Similarity Texter Blazor** è un'applicazione web avanzata che consente di confrontare due testi, evidenziando segmenti simili per facilitare attività come il rilevamento del plagio e l'analisi testuale. Basato sull'algoritmo **sim_text** di **Dick Grune**, il progetto integra le idee e il codice originale di **Similarity Texter** sviluppato da **Sofia Kalaidopoulou**.

---

## 🚧 **Stato del Progetto**

**Alpha Release**: In fase di sviluppo attivo. Alcune funzionalità potrebbero essere incomplete o soggette a modifiche.

---

## ⭐ **Caratteristiche**

- **Confronto Testuale Avanzato**: Utilizza l'algoritmo "sim_text" per identificare segmenti simili.
- **Interfaccia Intuitiva**: Sviluppata con Blazor per un'esperienza utente fluida e moderna.
- **Opzioni Configurabili**: Personalizza il confronto ignorando maiuscole/minuscole, numeri, punteggiatura e sostituendo gli umlaut.
- **Ampia Gamma di Input**: Supporta il caricamento di file `.txt` e l'inserimento manuale di testo.
- **Risultati Visivi**: Segmenti simili evidenziati direttamente nei testi caricati.
- **Esportazione e Stampa**: Genera riepiloghi e stampa i risultati del confronto.

---

## 🛠️ **Tecnologie Utilizzate**

![Blazor](https://img.shields.io/badge/Blazor-5.0-blue.svg) ![.NET](https://img.shields.io/badge/.NET-6.0+-purple.svg) ![xUnit](https://img.shields.io/badge/xUnit-2.4.1-blue.svg) ![Moq](https://img.shields.io/badge/Moq-4.16.1-green.svg)

- **Blazor**: Framework per la creazione di interfacce utente interattive.
- **.NET 6+**: Piattaforma di sviluppo per applicazioni web robuste.
- **Moq & xUnit**: Strumenti di testing per garantire qualità e affidabilità del codice.

---

## 🗂️ **Struttura del Progetto**

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

- **Application**: Logica principale dell'applicazione, inclusi confronto testuale e pipeline.
- **Domain**: Modelli di dominio e gestione dei riferimenti avanzati.
- **Infrastructure**: Implementazioni concrete dei servizi necessari.
- **Blazor**: Componenti dell'interfaccia utente sviluppati con Blazor.
- **OriginalSimtexterAppJs**: Codice e documentazione del progetto originale in JavaScript.
---
---

## 🚀 **Utilizzo**

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

---

## 📄 **Documentazione Dettagliata**

- [Architettura](./docs/architecture.md)
- [Algoritmo](./docs/algorithm.md)
- [Frontend con Blazor](./docs/frontend.md)
- [Strategie di Testing](./docs/testing.md)

---

## 📜 **Licenza**

Questo progetto è distribuito sotto la licenza [CC BY-NC-SA 4.0](LICENSE), in conformità con:

- **Dick Grune**: L'algoritmo "sim_text" è distribuito sotto licenza [BSD-3-Clause](https://opensource.org/licenses/BSD-3-Clause).
- **Sofia Kalaidopoulou**: Il progetto "Similarity Texter" è rilasciato sotto licenza [CC BY-NC-SA 4.0](https://creativecommons.org/licenses/by-nc-sa/4.0/).

I contributi di entrambi gli autori sono riconosciuti esplicitamente nel presente README.

---

## 🤝 **Riconoscimenti**

- **Dick Grune**: Creatore dell'algoritmo "sim_text". Maggiori informazioni su [dickgrune.com](https://dickgrune.com/Programs/similarity_text/).
- **Sofia Kalaidopoulou**: Autrice del progetto originale **Similarity Texter**. Visita [Similarity Texter](https://people.f4.htw-berlin.de/~weberwu/simtexter/app.html) per ulteriori dettagli.

---

## 📚 **Citazioni**

- **Dick Grune**: Grune, D. (1989). *sim_text: A text similarity tester*. Vrije Universiteit Amsterdam. Disponibile su [dickgrune.com](https://dickgrune.com/Programs/similarity_text/).
- **Sofia Kalaidopoulou**: Kalaidopoulou, S. (2016). *Similarity Texter: A text-comparison web tool based on the sim_text algorithm*. Tesi di laurea, HTW Berlin. Disponibile su [htw-berlin.de](https://people.f4.htw-berlin.de/~weberwu/simtexter/app.html).

---

## 🔮 **Miglioramenti Futuri**

- 📸 **Screenshot e Demo Video**: Aggiungi elementi visivi per migliorare la comprensione del progetto.
- ❓ **Sezione FAQ**: Risposte a domande comuni degli utenti.
- 📜 **Dettagli sulle Dipendenze**: Lista dettagliata delle dipendenze del progetto.
- 🖥️ **Integrazione CI/CD**: Implementare pipeline di integrazione e distribuzione continua.
- 🛡️ **Sicurezza**: Rafforzare le misure di sicurezza per la gestione dei dati degli utenti.

---

## 🆘 **Supporto**

Hai bisogno di aiuto o hai un suggerimento? [Crea un'issue su GitHub](https://github.com/slim16165/similarity-texter-blazor/issues) oppure contattami tramite [LinkedIn](https://www.linkedin.com/in/gianluigisalvi/) o [GitHub](https://github.com/slim16165/similarity-texter-blazor).

---

## 🎨 **Screenshot**

*Includi qui degli screenshot dell'applicazione per mostrare l'interfaccia utente e i risultati del confronto testuale.*

---

## 📈 **Demo**

*Puoi aggiungere un link a una demo live dell'applicazione o a un video dimostrativo.*

---

## 💡 **Best Practices**

- **Codice Pulito**: Mantieni il codice leggibile e ben documentato.
- **Test Automatizzati**: Utilizza xUnit e Moq per garantire la qualità del codice.
- **Gestione delle Dipendenze**: Assicurati che tutte le dipendenze siano aggiornate e ben gestite tramite `package.json` e altri strumenti di gestione.

