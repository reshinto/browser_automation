# Browser Automation Toolkit

## Overview
This repository contains two small Selenium-based utilities that automate the steps needed to submit a weekly report in Cybozu Garoon. Both implementations – one in Python and one in C# – read login details, element locators, and report content from local files, open Chrome through ChromeDriver, sign in, navigate to the weekly report form, and populate each section before optionally submitting the report. The scripts are designed so that non-developers can prepare a structured text file and let the automation handle the repetitive browser actions for them.

## Repository Structure
- `python_version/` – A Python 3.8 script (`run.py`) that orchestrates the automation with Selenium, loading configuration from JSON and parsing a text file to fill the report.
- `CSharp_version/` – A .NET Core 3.1 console application that mirrors the Python workflow using Selenium WebDriver and Newtonsoft.Json.
- Support files (`secrets.json`, `elements.json`, `weeklyReport.txt`, and optionally `originalWeeklyReport.txt`) are expected to live alongside the executable or to be reachable from the working directory, depending on the language version you run.

## Key Features
- **Cross-language automation**: Choose between Python and C# depending on your preferred runtime environment while keeping the same automation flow (`run.py`, `CSharp_version/WeeklyReportCSharp/WeeklyReport.cs`).
- **Configuration-driven login and navigation**: Credentials and element locators are externalized so the automation can adapt to layout changes without code edits (`secrets.json`, `elements.json`).
- **Structured weekly report parsing**: Report content is pulled from a tagged plain-text file that marks each section with headers such as `(subject)` and `(end)`, allowing multiline entries without manual form filling (`run.py`, `WeeklyReport.cs`).
- **Resilient browser actions**: Repeated attempts are made when clicking or typing until the element becomes available, reducing flakiness caused by page loads (`run.py`, `WeeklyReport.cs`).
- **Optional submission control**: The Python script separates data entry from submission, and the C# application asks for confirmation before sending the form, letting you review the populated page first (`run.py`, `Program.cs`).
- **Automatic date labelling**: The subject line automatically includes the current week range, keeping report titles consistent without manual calculation (`run.py`, `WeeklyReport.cs`).
- **Report reset helper (C#)**: After a successful submission, the C# workflow can restore the report template from `originalWeeklyReport.txt` so it is ready for the next run (`WeeklyReport.cs`, `Constants.cs`).

## Required Data Files
Create the following files before running either implementation:

- `secrets.json` – Stores credentials grouped by site name (for example `"garoon"`).
  ```json
  {
    "garoon": {
      "username": "myUsername",
      "password": "myPassword"
    }
  }
  ```
- `elements.json` – Holds XPath locators for every element the automation touches. Names should follow the sections used in the scripts (`login`, `links`, `weeklyReport`, etc.).
  ```json
  {
    "url": "https://example.com/login",
    "garoon": {
      "login": {
        "userInput": "//input[@id='username']",
        "pwInput": "//input[@id='password']",
        "button": "//button[@type='submit']"
      },
      "links": {
        "multiReport": "//a[@id='reports']",
        "prepareReport": "//a[@id='prepare']",
        "weeklyReport": "//a[@id='weekly']"
      },
      "weeklyReport": {
        "subjectInput": "//input[@id='subject']",
        "workThisWeekInput": "//textarea[@id='work']",
        "problemsInput": "//textarea[@id='problems']",
        "planForNextWeekInput": "//textarea[@id='plans']",
        "infoToManagerInput": "//textarea[@id='info']",
        "impressionInput": "//textarea[@id='impression']",
        "notiForBossSelect": "//select[@id='boss']",
        "notiAddButton": "//button[@id='add']",
        "confirmDetailsButton": "//button[@id='confirm']",
        "submitButton": "//button[@id='submit']"
      }
    }
  }
  ```
- `weeklyReport.txt` – Contains the actual report content. Each section begins with a header such as `(work)` and ends with `(end)`. The automation appends the appropriate lines to the matching form fields.
  ```
  (subject)
  Weekly Report
  (end)
  (work)
  - Completed feature X
  - Reviewed pull requests
  (end)
  ```
- `originalWeeklyReport.txt` (C# only, optional) – A pristine copy of the report template that is copied back over `weeklyReport.txt` after submission when `Constants.overwrite` is `true`.

## Prerequisites
- Google Chrome installed and a matching ChromeDriver available on your `PATH`.
- Access to Cybozu Garoon (or any target site that matches the structure described in `elements.json`).
- For the Python version:
  - Python 3.8+
  - Pipenv (for managing dependencies)
- For the C# version:
  - .NET Core SDK 3.1+

## Getting Started (Python)
1. Navigate to `python_version/` and install dependencies:
   ```bash
   cd python_version
   pipenv install
   ```
2. Place `secrets.json`, `elements.json`, and your `weeklyReport.txt` in the same directory (or adjust the paths in the script if you store them elsewhere).
3. Update the path passed to `readWeeklyReport` at the bottom of `run.py` so it points to your `weeklyReport.txt`.
4. Launch the automation:
   ```bash
   pipenv run python run.py
   ```
5. The script opens Chrome, performs the login and navigation steps, fills in the report fields, and leaves the page open so you can verify the content. Uncomment the `submit()` call in `run.py` if you want the Python version to submit automatically.

## Getting Started (C#)
1. Navigate to `CSharp_version/WeeklyReportCSharp` and restore dependencies:
   ```bash
   cd CSharp_version/WeeklyReportCSharp
   dotnet restore
   ```
2. Ensure `secrets.json`, `elements.json`, `weeklyReport.txt`, and (optionally) `originalWeeklyReport.txt` reside in the same folder where the compiled executable will run. The defaults in `Constants.cs` expect them beside the application.
3. Build and run the console app:
   ```bash
   dotnet run
   ```
4. After the form is populated, the application asks whether to submit. Enter `y` to send the report (it will also reset `weeklyReport.txt` from `originalWeeklyReport.txt` if available) or any other key to close the browser without submitting.

## Customising and Extending
- Adjust `elements.json` whenever the page structure changes. Because every XPath is externalized, you do not need to modify the source code for most UI tweaks.
- Add new report sections by extending the `_inputs` dictionary in the respective language and creating matching tags in `weeklyReport.txt`.
- Wrap ChromeDriver options (headless mode, profiles, etc.) by editing the WebDriver initialization in `run.py` or `WeeklyReport.cs` if you need to run the automation in different environments.

With these pieces in place, the repository gives you a repeatable way to transform a simple text file into a fully populated browser-based weekly report with minimal manual effort.
