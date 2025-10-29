# Vosk Welsh Speech Recognition (Avalonia - Cross-Platform)

A cross-platform GUI application for real-time Welsh (Cymraeg) speech recognition using Vosk. Built with Avalonia UI, it runs on **macOS, Windows, and Linux**.

## Quick Start

### 1. Build the Application

```bash
cd VoskWelshSpeechRecognitionAvalonia
dotnet restore
dotnet build
```

### 2. Download the Welsh Model

Download the Welsh Vosk model from:
- **GitHub**: https://github.com/Cymru-Breizh-Agile-Cymru-Project/vosk-cymraeg
- **Hugging Face**: https://huggingface.co/techiaith/kaldi-cy

Extract and place in:
```
VoskWelshSpeechRecognitionAvalonia/bin/Debug/net8.0/Models/vosk-model-cy/
```

### 3. Run the Application

```bash
dotnet run
```

Or run the executable directly:
```bash
./bin/Debug/net8.0/VoskWelshSpeechRecognitionAvalonia
```

## Features

- ✅ **Cross-Platform**: Runs on macOS, Windows, and Linux
- ✅ **Real-time Speech Recognition**: Live microphone input
- ✅ **Bilingual Interface**: Welsh and English labels
- ✅ **Offline Processing**: No internet required after model download
- ✅ **Live Partial Results**: See transcription as you speak
- ✅ **Timestamped Transcriptions**: Final results with timestamps

## Platform-Specific Notes

### macOS
- Works perfectly on macOS (tested on ARM and Intel)
- Microphone permissions may be required (System Preferences → Security & Privacy)

### Windows
- Fully compatible
- Alternative to the WPF version with the same functionality

### Linux
- Requires `libportaudio` for microphone support:
  ```bash
  # Ubuntu/Debian
  sudo apt install libportaudio2

  # Fedora
  sudo dnf install portaudio

  # Arch
  sudo pacman -S portaudio
  ```

## Dependencies

- **Avalonia UI** 11.3.6 - Cross-platform UI framework
- **Vosk** 0.3.38 - Offline speech recognition
- **NAudio** 2.2.1 - Audio capture
- **Newtonsoft.Json** 13.0.3 - JSON parsing

## Usage

1. **Start Recording**: Click "Start Recording / Dechrau Recordio"
2. **Speak Welsh**: Live partial transcription appears in yellow panel
3. **View Results**: Completed sentences show in white panel with timestamps
4. **Stop Recording**: Click "Stop Recording / Stopio Recordio"
5. **Clear**: Click "Clear / Clirio" to reset panels

## Building for Production

### Create Self-Contained Executable

**macOS (ARM)**:
```bash
dotnet publish -c Release -r osx-arm64 --self-contained
```

**macOS (Intel)**:
```bash
dotnet publish -c Release -r osx-x64 --self-contained
```

**Windows**:
```bash
dotnet publish -c Release -r win-x64 --self-contained
```

**Linux**:
```bash
dotnet publish -c Release -r linux-x64 --self-contained
```

The executable will be in: `bin/Release/net8.0/{runtime}/publish/`

## Differences from WPF Version

| Feature | WPF Version | Avalonia Version |
|---------|-------------|------------------|
| **Platform** | Windows only | macOS, Windows, Linux |
| **UI Framework** | WPF | Avalonia UI |
| **Development** | Requires Windows | Develop anywhere |
| **Functionality** | Identical | Identical |

## Troubleshooting

### "Model not found!" on startup
- Ensure model is at: `bin/Debug/net8.0/Models/vosk-model-cy/`
- Check folder name is exactly `vosk-model-cy`

### No microphone input
- **macOS**: Grant microphone permissions in System Preferences
- **Windows**: Check Privacy settings
- **Linux**: Ensure `libportaudio` is installed

### Build errors
```bash
dotnet clean
dotnet restore
dotnet build
```

## Project Structure

```
VoskWelshSpeechRecognitionAvalonia/
├── VoskWelshSpeechRecognitionAvalonia.csproj  # Project file
├── MainWindow.axaml                            # UI layout
├── MainWindow.axaml.cs                         # Application logic
├── App.axaml                                   # Application definition
├── App.axaml.cs                                # Application startup
├── Models/                                     # Place model here
│   └── vosk-model-cy/                         # Welsh model (download)
└── README.md                                   # This file
```

## Credits

- **Vosk**: Alpha Cephei Inc. - https://alphacephei.com/vosk/
- **Avalonia UI**: Avalonia Team - https://avaloniaui.net/
- **NAudio**: Mark Heath
- **Welsh Models**: Cymru-Breizh-Agile Cymru Project
- **Inspiration**: vosk-tui Python implementation

## License

Application code for educational use. Check component licenses:
- **Vosk**: Apache 2.0
- **Avalonia**: MIT
- **NAudio**: MIT
- **Welsh Models**: CC-BY-NC-3.0

---

**Pob lwc! (Good luck!)**
