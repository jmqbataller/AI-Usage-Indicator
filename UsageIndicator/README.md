# ChatGPT Usage Indicator (Windows)

A small, always-on-top Windows widget for showing your remaining **Codex** and **ChatGPT Work** usage, plus each reset time.

## What it does

- starts as a compact indicator; click it to expand or collapse the two usage meters;
- stays above other windows and can be dragged anywhere on the screen;
- remembers its position and the values you entered;
- displays separate Codex and ChatGPT Work meters;
- stores its settings only in `%AppData%\\ChatGPTUsageIndicator\\settings.json`.

## Important limitation

ChatGPT Work/Codex usage is account data shown in the official usage dashboard. This app deliberately does **not** read browser cookies, saved passwords, or a private/internal ChatGPT endpoint. Update the two values from the dashboard using **Edit**. That means the indicator is accurate when you update it and safe to use on a work PC.

## Build on Windows

1. Install the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).
2. Open a terminal inside this folder.
3. Run:

```powershell
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o .\publish
```

4. Open `publish\UsageIndicator.exe`.

To launch it with Windows, create a shortcut to the EXE and place it in `shell:startup`.

## How to use

1. Open the widget. It starts as a slim floating bar at the top of the screen.
2. **Click the bar** to expand it. Drag the bar instead when you want to move it.
3. Click **Edit** and copy the remaining percentages and reset information from the official ChatGPT/Codex usage screen.
4. Click **Save**. The compact bar and expanded meters update immediately.
5. Click the widget again to collapse it. Your values and its position are remembered the next time it opens.

## Create the Windows EXE from GitHub

This repository includes a GitHub Actions workflow. Open the repository's **Actions** tab, open the newest **Build Windows app** run, and download the `AI-Usage-Indicator-win-x64` artifact. Extract it and run `UsageIndicator.exe`.
