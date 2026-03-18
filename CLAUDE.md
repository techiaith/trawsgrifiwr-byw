# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Trawsgrifiwr Byw** (Live Transcriber) is a cross-platform desktop application for real-time Welsh speech recognition. Built with C#/.NET 8.0 and Avalonia UI, it runs on Windows, macOS, and Linux. The UI is bilingual (Welsh/English).

## Commands

### Development
```bash
cd src
dotnet restore
dotnet build
dotnet run
```

### Production Builds
```bash
./build-macos.sh       # Creates .app bundle in dist/macos-osx-{arm64|x64}/
./build-windows.sh     # Creates exe in dist/windows-x64/
./build-linux.sh       # Creates executable in dist/linux-x64/
./build-all.sh         # Builds all three platforms
./create-releases.sh   # Packages builds as ZIP/TAR.GZ archives in releases/
./build-installer.sh   # Windows MSI only; requires WiX Toolset 3.11+
```

### Releases
Tag a commit to trigger GitHub Actions release build across all platforms:
```bash
git tag -a v1.0.0 -m "Release v1.0.0"
git push origin v1.0.0
```

## Architecture

All application logic lives in `src/MainWindow.axaml.cs` (code-behind pattern, not MVVM).

### Audio Pipeline (threaded)
1. **OpenAL capture** (`CaptureAudioLoop`) runs on a background thread, reads 4096-sample PCM frames at 16kHz mono 16-bit
2. Frames are pushed to a `BlockingCollection<short[]>` (capacity 100) as a thread-safe queue
3. **Vosk recognizer** (`ProcessAudioQueue`) runs on a second background thread, consumes the queue
4. Partial results update the live transcription display; final results append timestamped utterances

### Model Management
- Welsh language model (~47MB) is downloaded from HuggingFace (`techiaith/kaldi-cy`) on first run
- Stored in `~/.local/Trawsgrifiwr-Byw/Models/` (Linux/macOS) or equivalent AppData (Windows)
- `DownloadAndExtractModel()` uses SharpZipLib for TAR.GZ decompression
- Download errors are logged to `download-error.log` in the same directory

### UI
- Avalonia XAML with Welsh color scheme (green `#00843D`, red `#C8102E`)
- Status indicator changes color: orange = initializing, red = recording, green = ready
- Real-time partial transcript display + completed utterances with timestamps
- Keyboard shortcuts: F5 (start), F6 (stop), F7 (copy without timestamps), F8 (copy with timestamps), F9 (clear)
- ARIA live regions and `AutomationProperties` for screen reader support

### Cross-Platform Notes
- Avalonia abstracts UI; OpenAL abstracts audio capture — no platform-specific audio code
- Build scripts set the correct RID (`osx-arm64`, `osx-x64`, `win-x64`, `linux-x64`)
- macOS builds create a full `.app` bundle with `Info.plist` and ad-hoc code signing
- All builds are self-contained single-file executables (includes .NET runtime)

## Key Files
- `src/MainWindow.axaml.cs` — all application logic (audio capture, model loading, transcription, UI updates)
- `src/MainWindow.axaml` — UI layout and styles
- `src/VoskWelshSpeechRecognition.csproj` — project file with all NuGet dependencies
- `.github/workflows/build-release.yml` — release CI/CD (triggered by `v*` tags)
- `.github/workflows/build-test.yml` — test build CI/CD (every push)

## Dependencies
- **Avalonia 11.3.6** — cross-platform UI framework
- **Vosk 0.3.38** — offline speech recognition engine
- **OpenTK.OpenAL 4.7.7** — cross-platform audio capture
- **NAudio 2.2.1** — Windows audio (supplementary)
- **SharpZipLib 1.4.2** — TAR.GZ extraction for model download
- **Newtonsoft.Json 13.0.3** — Vosk result JSON parsing
