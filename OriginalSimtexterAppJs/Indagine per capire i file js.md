# OriginalSimtexterAppJs


# Analisi dei file JavaScript: Bundling e Relazioni

## **Obiettivo del Progetto**
Dimostrare che il file `app.js` contiene il codice presente nei file della directory `Js Unpacked`, nonostante alterazioni strutturali causate dal processo di bundling, minificazione e ottimizzazione.

---

## **Struttura dei File Analizzati**

### File principali:
- `app.js`: Punto d'ingresso principale dell'applicazione, contenente il codice presumibilmente bundlato.
- `app.min.js`: Versione minificata di `app.js`. Tuttavia, differenze strutturali indicano che non è una semplice versione compressa.
- `TextCompare.js`: File simile ad `app.js`, con molte dipendenze comuni.

### File unbundled (nella directory `Js Unpacked`):
- Contengono moduli indipendenti utilizzati nell'applicazione, tra cui:
  - Gestione input: `fileInputReader.js`, `textInputReader.js`
  - Logica centrale: `simtexter.js`, `match.js`, `matchSegment.js`
  - Interfaccia utente: `view.js`, `template.js`
  - Persistenza: `storage.js`

---

## **Approcci Utilizzati**

### 1. **Confronto diretto dei contenuti**
- **Metodo**: Normalizzazione dei file (rimozione di commenti, spaziature) e confronto diretto.
- **Risultati**:
  - Somiglianze basse (7%-37%) tra `app.js` e i file di `Js Unpacked`.
  - Indicazione che i file sono stati significativamente modificati durante il bundling.

### 2. **Trasformata di Fourier**
- **Metodo**: Conversione dei file in sequenze numeriche (valori ASCII dei caratteri), applicazione della trasformata di Fourier e confronto delle frequenze spettrali.
- **Risultati**:
  - Correlazioni elevate (96%-98%) tra `app.js` e i file unbundled, suggerendo inclusione o sovrapposizione significativa.

### 3. **Verifica manuale delle somiglianze**
- **Metodo**: Confronto diretto tra sezioni di `app.js` e i file unbundled con algoritmi di similarità (SequenceMatcher).
- **Risultati**:
  - Inclusione parziale confermata, ma con trasformazioni che impediscono un riconoscimento diretto completo.

---

## **Problemi Incontrati**
- **Alterazioni Strutturali**:
  - Funzioni rinominate e blocchi riordinati durante il bundling.
  - Tree-shaking e ottimizzazioni aggressive che rimuovono codice inutilizzato.
- **Limitazioni degli approcci tradizionali**:
  - Tecniche di confronto diretto non hanno catturato le somiglianze a causa delle trasformazioni.

---

## **Prossimi Passi**
1. **Adottare Tecniche Avanzate**:
   - Fingerprinting del codice (es. K-Shingling) per identificare frammenti di codice comuni.
   - Confronto delle AST (Abstract Syntax Tree) per catturare la logica, indipendentemente dalle modifiche superficiali.
2. **Collaborare con LLM**:
   - Richiedere strategie avanzate per dimostrare l'inclusione dei file unbundled in `app.js`.

---

## **Conclusioni Preliminari**
- È altamente probabile che `app.js` contenga i file unbundled.
- Tuttavia, le trasformazioni durante il bundling rendono difficile dimostrarlo con tecniche di confronto convenzionali.

Questo progetto continuerà con un'analisi approfondita per fornire prove concrete.
