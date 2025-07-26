# 📚 Quote Finder – Ultimate C# Masterclass Assignment

A console application that searches for quotes containing a specific word from the Quote Garden API (or a mock data source), with support for **sequential** and **parallel processing**, performance timing, and smart word matching.

---

## 📍 Navigation

- [🎯 Project Overview](#-project-overview)
- [🛠️ Features](#-features)
- [📦 Technologies Used](#-technologies-used)
- [📁 Project Structure](#-project-structure)
- [🧩 How It Works](#-how-it-works)
- [🧪 Example Run](#-example-run)
- [🔌 Interface Definition](#-interface-definition)
- [⚙️ Building and Running](#-building-and-running)

---

## 🎯 Project Overview

The **Quote Finder** app allows users to:

- Search for a specific **word** in quotes from the [Quote Garden API](https://pprathameshmore.github.io/QuoteGarden/).
- Specify the number of **pages** to retrieve and **quotes per page**.
- Choose between **sequential (single-threaded)** or **parallel (multi-threaded)** processing.
- View results with proper console formatting and execution time.

> ✅ Uses **mock data** via `MockQuotesApiDataReader` to ensure reliability even if the external API is down.

---

## 🛠️ Features

| Feature | Description |
|-------|-------------|
| 🔍 **Smart Word Matching** | Matches whole words only (e.g., "cat" won't match "category") |
| 📄 **Pagination Support** | Fetches user-defined number of pages with custom quote limit per page |
| ⏱️ **Execution Time Measurement** | Uses `Stopwatch` to compare performance between modes |
| 🔁 **Sequential & Parallel Modes** | User chooses processing mode (y/n) |
| 🧪 **Mock API Data Reader** | Implements `IQuotesApiDataReader` for stable offline testing |
| ✅ **Input Validation** | Ensures word is alphabetic, numbers are positive, and choices are valid |
| 💡 **Color-Coded Console Output** | Green = success, red = error, white = info |

---

## 📦 Technologies Used

- **C# (.NET 6 or later)**
- **Console Application**
- **System.Text.Json** – JSON deserialization
- **Tasks & Threading** – Asynchronous and parallel execution
- **Interfaces** – Clean separation via `IQuotesApiDataReader`, `IUserPrompts`, etc.
- **Stopwatch** – High-resolution timing

---

## 📁 Project Structure

```
QuoteFinder/
│
├── Application.cs          // Main workflow and user interaction
├── UserPrompts.cs         // Handles console messages with colors
├── SingleThreading.cs     // Sequential processing logic
├── ProcessAsync.cs        // Parallel processing using Task.Run
├── MockQuotesApiDataReader.cs // Mocks API responses using embedded JSON
├── Models/
│   └── QuoteModels.cs     // DTOs: Root, Datum, etc.
```

> 🔗 **API Endpoint (for reference):**  
> `https://quote-garden.onrender.com/api/v3/quotes?limit={n}&page={n}`  
> 💡 **Mocked**: The app uses `MockQuotesApiDataReader` to simulate real API responses without network dependency.

---

## 🧩 How It Works

### 1. User Input
The app prompts for:
- **Word to search**: Must be a single alphabetic word (e.g., "life")
- **Number of pages**: How many pages of quotes to fetch
- **Quotes per page**: Limit of quotes per API call (max 100 recommended)
- **Parallel processing?**: `y` for parallel, `n` for sequential

### 2. Data Retrieval
- One call per page using `ReadAsync(page, quotesPerPage)`
- Response is a JSON string containing a list of quotes

### 3. Processing Logic
- Deserializes JSON into `Root` object
- Searches for quotes containing the **exact word** (case-insensitive, whole word only)
- If multiple matches: selects the **shortest quote**
- If no match: prints "no quote is found on a page"

### 4. Output
- Prints result per page with thread ID
- Shows total execution time in milliseconds
- Ends with "Program is finished." and waits for key press

---

## 🧪 Example Run

```
What word are you looking for?
life
Number of pages you want:
2
How many quotes you want per page:
5
Should processing be performed in parallel? (y/n)
y
Fetching Data...
DataPrompt Id: 11
"Life is what happens when you're busy making other plans." – John Lennon
DataPrompt Id: 12
You only live once, but if you do it right, once is enough. – Mae West
Program is finished.
The Time For Your Execution Is: 1456
```

---

## 🔌 Interface Definition

```csharp
namespace _16_QuoteFinder.DataAccess;

public interface IQuotesApiDataReader : IDisposable
{
    Task<string> ReadAsync(int page, int quotesPerPage);
}
```

Implemented by `MockQuotesApiDataReader` to return realistic mock JSON data for testing.

---

## ⚙️ Building and Running

### Prerequisites
- **.NET 6 SDK** or higher
- Any IDE (Visual Studio, VS Code, Rider) or use CLI

### Steps
1. Open terminal in project directory
2. Build the project:
   ```bash
   dotnet build
   ```
3. Run the app:
   ```bash
   dotnet run
   ```
