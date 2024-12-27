## Similarity Texter: A Text-Comparison Web Tool Based on the Sim_Text Algorithm

### Author of the original work
**Sofia Kalaidopoulou**

### Author of this resume
**Gianluigi Salvi**


### Thesis Overview
This Bachelor Thesis presents the development of a web tool called **Similarity Texter**, which is designed for text comparison and is based on the `sim_text` algorithm originally developed by Dick Grune in 1989. The tool focuses on detecting lexical similarities and providing a user-friendly interface for tasks like plagiarism detection and other text comparison needs.

---

## Table of Contents
1. [Abstract](#abstract)
2. [Sim_Text Algorithm](#sim_text-algorithm)
3. [Similarity Texter Features](#similarity-texter-features)
4. [Development and Technical Overview](#development-and-technical-overview)
5. [Usage Instructions](#usage-instructions)

---

### Abstract
Text comparison is a critical domain in computer science, applicable in fields like plagiarism detection, information retrieval, and DNA sequence analysis. This thesis explores the implementation of the `sim_text` algorithm in a web-based application that highlights the longest common substrings between two input texts. The project bridges the gap between the technical efficiency of desktop tools and the accessibility of web applications.

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
- Designed to provide a graphical user interface (GUI) for enhanced user experience.

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
- Based on the Gestalt principles of perception for intuitive layout.
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

### Conclusion
**Similarity Texter** modernizes the proven efficiency of the `sim_text` algorithm, offering an accessible, web-based solution for text comparison. It addresses the limitations of traditional console-based tools by integrating advanced visualization and interaction features.

