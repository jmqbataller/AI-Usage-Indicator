# ChatGPT Usage Indicator (Windows)

A small, always-on-top Windows widget for monitoring the **Work shared-plan** usage from the signed-in ChatGPT page.

## What it does

- starts as a compact indicator; click it to expand or collapse the Work meter;
- stays above other windows and can be dragged anywhere on the screen;
- supports separate sign-ins for multiple ChatGPT accounts;
- opens the plan usage screen after sign-in and refreshes it in the background;
- stores browser profiles and detected values only in `%LocalAppData%\\AIUsageIndicator`.

## Important limitation

ChatGPT usage is account data shown in the official dashboard. Sign in manually through the app's ChatGPT page; the app then reads only the text visibly rendered in that browser panel. It does not have a password field or call undocumented/private ChatGPT endpoints.

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
3. Click **+ Account**, then sign in on the official ChatGPT page.
4. After sign-in, the app opens the plan usage screen automatically and detects the percentage and reset time.
5. Add another account when needed. Every account has its own local signed-in browser profile.

## Create the Windows EXE from GitHub

This repository includes a GitHub Actions workflow. Open the repository's **Actions** tab, open the newest **Build Windows app** run, and download the `AI-Usage-Indicator-win-x64` artifact. Extract it and run `UsageIndicator.exe`.
