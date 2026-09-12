# AI Usage Indicator

A lightweight Windows floating widget for monitoring the **Work shared-plan** usage shown by your signed-in ChatGPT accounts.

![Platform](https://img.shields.io/badge/platform-Windows-0078D6?logo=windows&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)

## Features

- Compact, always-on-top usage bar that floats above your work.
- Click the bar to expand and view the live Work shared-plan meter.
- Drag it anywhere on the screen.
- Add multiple ChatGPT account profiles and sign in to each one directly inside the app.
- Opens the plan usage screen automatically after a successful sign-in and refreshes it in the background.
- Each account keeps a separate local Microsoft Edge WebView profile, so sessions do not mix.
- No password field is created or handled by this app; the sign-in page is ChatGPT's own page.
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
3. Select **+ Account**, then sign in on the official ChatGPT page inside the app.
4. After the sign-in succeeds, the app automatically opens the plan usage screen in the background and detects the current percentage and next reset.
5. The login page disappears after the plan is detected. Select **+ Account** again for each additional account.
6. Click the bar again to collapse it. Your account sessions and last detected values remain available on the same PC.

To start it automatically with Windows, place a shortcut to `UsageIndicator.exe` in `shell:startup`.

## Build from source

Install the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0), then run:

```powershell
dotnet publish UsageIndicator/UsageIndicator.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o .\release
```

The resulting application is `release\UsageIndicator.exe`.

## Privacy and accuracy

The official usage dashboard remains the source of truth. This app uses the content visibly rendered in its own signed-in browser panel; it does not use undocumented ChatGPT APIs. A token count is shown only when ChatGPT visibly supplies one. Model-based hours are not calculated because ChatGPT usage allowances are not a fixed number of tokens per hour.
