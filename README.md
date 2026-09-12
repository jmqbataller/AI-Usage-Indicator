# AI Usage Indicator

A lightweight Windows floating widget for tracking your remaining **Codex** and **ChatGPT Work** usage in one place.

![Platform](https://img.shields.io/badge/platform-Windows-0078D6?logo=windows&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)

## Features

- Compact, always-on-top usage bar that floats above your work.
- Click the bar to expand and view separate Codex and ChatGPT Work meters.
- Drag it anywhere on the screen.
- Store each remaining percentage and reset note locally on the computer.
- No browser cookies, ChatGPT passwords, or private endpoints are read.
- GitHub Actions creates a downloadable, self-contained Windows build after every push.

## Download and run

1. Open the repository's **Actions** tab.
2. Select the newest **Build Windows app** run.
3. Under **Artifacts**, download `AI-Usage-Indicator-win-x64`.
4. Extract it and run `UsageIndicator.exe`.

Windows may display a SmartScreen notice because the app is new and unsigned. Select **More info** → **Run anyway** only after downloading it from this repository.

## How to use

1. Open `UsageIndicator.exe`. A slim bar appears at the top of the screen.
2. Click the bar to expand it; drag it to reposition it.
3. Select **Edit**.
4. Enter the remaining percentage and reset information shown on your official ChatGPT/Codex usage screen.
5. Select **Save**. The small bar and expanded meters refresh immediately.
6. Click the bar again to collapse it. Its location and values are remembered.

To start it automatically with Windows, place a shortcut to `UsageIndicator.exe` in `shell:startup`.

## Build from source

Install the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0), then run:

```powershell
dotnet publish UsageIndicator/UsageIndicator.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o .\release
```

The resulting application is `release\UsageIndicator.exe`.

## Privacy and accuracy

The official usage dashboard is the source of truth. This app intentionally does not access browser sign-in data, saved passwords, cookies, or undocumented ChatGPT endpoints. Update the two values whenever you want the widget to reflect the latest dashboard values.
