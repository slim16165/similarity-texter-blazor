## Similarity Texter: A Text-Comparison Web Tool Based on the Sim_Text Algorithm

### Author of the Original Work
**Sofia Kalaidopoulou**

### Author of This Summary
**Gianluigi Salvi**

---

## 📚 Table of Contents
1. [Abstract](#abstract)
2. [Sim_Text Algorithm](#sim_text-algorithm)
3. [Similarity Texter Features](#similarity-texter-features)
4. [Development and Technical Overview](#development-and-technical-overview)
5. [Usage Instructions](#usage-instructions)
6. [Project Structure](#project-structure)
7. [Main Files](#main-files)
8. [Source Folder Content](#source-folder-content)
9. [Conclusion](#conclusion)

---

### Abstract
Text comparison is a critical domain in computer science, applicable in fields such as plagiarism detection, information retrieval, and DNA sequence analysis. This thesis explores the implementation of the `sim_text` algorithm in a web-based application that highlights the longest common substrings between two input texts. The project bridges the gap between the technical efficiency of desktop tools and the accessibility of web applications.

---

### Sim_Text Algorithm

**Background:**
- Developed for detecting similarities in natural language and source code.
- Utilized in plagiarism detection and software refactoring.
- Efficient but designed as a console-based tool, limiting its usability for non-technical users.

**Key Features:**
- Tokenizes input texts.
- Constructs a forward reference table for comparisons.
- Identifies the longest common substrings (LCS) between texts.

---

### Similarity Texter Features

**Concept:**
- A web-based adaptation of the `sim_text` algorithm.
- Designed to provide a graphical user interface (GUI) for an enhanced user experience.

**Functional Requirements:**
1. **Input Options:** Supports various file types, including DOCX, ODT, TXT, and plain/HTML text.
2. **Comparison Options:** Allows fine-tuning of comparison parameters.
3. **Visualization:** Highlights matches in a side-by-side display.
4. **Export Functionality:** Generates PDF reports of comparison results.
5. **Interactive Features:** Enables auto-scrolling to matched sections for ease of review.

---

### Development and Technical Overview

**Development Language:**
- Implemented in JavaScript for client-side execution.

**GUI Design:**
- Based on the Gestalt principles of perception for an intuitive layout.
- Incorporates responsive design for various devices and screen sizes.

**Core Libraries and Tools:**
- **Bootstrap:** For styling and responsive design.
- **jQuery:** Simplifies DOM manipulation.
- **JSZip:** For processing DOCX and ODT files.
- **Browserify:** Bundles JavaScript dependencies.

**Algorithm Implementation:**
- Adopts the `sim_text` algorithm’s hashing and tokenization methods.
- Ensures efficient comparison with forward reference and hashing tables.

---

### Usage Instructions

1. **Input:**
   - Upload two files or paste text for comparison.
   - Select file types and encoding (supports UTF-8).

2. **Set Options:**
   - Customize parameters like minimum match size or output format.

3. **Run Comparison:**
   - Initiate the process to highlight and analyze textual similarities.

4. **Review Results:**
   - Examine matches in the side-by-side display.
   - Use auto-scroll to navigate to specific matches.

5. **Export Results:**
   - Generate a detailed PDF report including matches and optional comments.

---

### Project Structure

```
OriginalSimtexterAppJs/
├── app.js                 # Main file for the original app's frontend
├── app.min.js             # Minified version of app.js
├── src/                   # Contains JavaScript code divided into modules
│   ├── js/
│   │   ├── app/           # Main application modules
│   │   ├── autoScroll/    # Manages automatic scrolling
│   │   ├── inputReader/   # Handles user input
│   │   └── simtexter/     # Core algorithm logic
├── webpack.config.js      # Configuration for building with Webpack
├── package.json           # Project dependency management
├── readme.md              # Original documentation
└── 522789_Sofia-Kalaidopoulou_bachelor-thesis.pdf # Reference thesis
```

---

### Main Files

#### 1. app.js
- **Function:** Entry point of the JavaScript application.
- **Contains:**
  - Logic for managing the user interface.
  - Links to internal modules (defined in `src/js/`).

#### 2. app.min.js
- **Function:** Optimized (minified) version of `app.js`.
- **Used for:** Production environments to enhance performance.

#### 3. webpack.config.js
- **Function:** Configures the build process.
- **Specifications:** How to combine modular JavaScript files in `src/js/` into a single file.

#### 4. package.json
- **Function:** Manages project dependencies.
- **Main Dependencies:**
  - Libraries like Webpack, Babel, etc.
- **Contains:** Scripts for building and other tasks.

#### 5. 522789_Sofia-Kalaidopoulou_bachelor-thesis.pdf
- **Function:** Reference document explaining the functionality and motivation of the original project.

---

### Source Folder Content

```
src/js/
├── app/                 # Main application modules
│   ├── app.js           # Entry point for app logic
│   ├── controller.js    # Manages user interaction
│   ├── inputText.js     # Module for text management
│   ├── storage.js       # Manages local storage
│   ├── template.js      # Utilities for handling UI templates
│   └── view.js          # Renders the UI
├── autoScroll/          # Modules for managing scrolling
│   ├── scrollPosition.js
│   └── targetMatch.js
├── inputReader/         # Modules for loading and reading text
│   ├── fileInputReader.js
│   └── textInputReader.js
└── simtexter/           # Core algorithm logic
    ├── match.js         # Function to identify similarities
    ├── matchSegment.js  # Manages similar segments
    ├── simtexter.js     # Entry point for comparison
    ├── text.js          # Utilities for text manipulation
    └── token.js         # Text tokenization
```

# License

- **Sofia Kalaidopoulou:** The "Similarity Texter" project is released under the [CC BY-NC-SA 4.0](https://creativecommons.org/licenses/by-nc-sa/4.0/) license.

---

### Conclusion
**Similarity Texter** modernizes the proven efficiency of the `sim_text` algorithm by offering an accessible, web-based solution for text comparison. It addresses the limitations of traditional console-based tools by integrating advanced visualization and interaction features, making it suitable for a broader range of users and applications.