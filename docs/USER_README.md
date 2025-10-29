# Vosk Welsh Speech Recognition / Adnabod Lleferydd Cymraeg

A cross-platform desktop application for real-time Welsh speech recognition using the Vosk offline speech recognition engine.

![Platform](https://img.shields.io/badge/platform-macOS%20%7C%20Windows%20%7C%20Linux-blue)
![.NET](https://img.shields.io/badge/.NET-8.0-purple)
![License](https://img.shields.io/badge/license-MIT-green)

## Features

- ✅ **Offline Speech Recognition** - No internet required after initial model download
- ✅ **Welsh Language Support** - Uses Kaldi Welsh models from [techiaith](https://huggingface.co/techiaith/kaldi-cy)
- ✅ **Real-time Transcription** - See partial results as you speak
- ✅ **Cross-platform** - Works on macOS, Windows, and Linux
- ✅ **Automatic Model Download** - Welsh model downloads automatically on first run (~47MB)
- ✅ **Bilingual UI** - Interface in Welsh and English

## Screenshots

```
┌─────────────────────────────────────────────────────┐
│   Vosk Welsh Speech Recognition                     │
├─────────────────────────────────────────────────────┤
│ Status: Recording... / Yn recordio...          🔴   │
├─────────────────────────────────────────────────────┤
│ [Start Recording]  [Stop Recording]  [Clear]        │
├─────────────────────────────────────────────────────┤
│ Live Input - Mewnbwn Byw                            │
│ ┌─────────────────────────────────────────────────┐ │
│ │ mae'n hen wlad fy nhadau...                     │ │
│ └─────────────────────────────────────────────────┘ │
├─────────────────────────────────────────────────────┤
│ Completed Transcriptions - Trawsgrifiadau Cyflawn  │
│ ┌─────────────────────────────────────────────────┐ │
│ │ [12:34:56] mae'n hen wlad fy nhadau             │ │
│ │ [12:35:02] yn annwyl i mi                       │ │
│ └─────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────┘
```

## Download & Installation

### macOS

**Requirements**: macOS 10.15 or later

1. Download the appropriate version:
   - [VoskWelshSpeechRecognition-macOS-osx-arm64-v1.0.0.zip](../../releases) - for Apple Silicon (M1/M2/M3)
   - [VoskWelshSpeechRecognition-macOS-osx-x64-v1.0.0.zip](../../releases) - for Intel Macs

2. Extract the zip file

3. **First time only**: Right-click `VoskWelshSpeechRecognition` → **Open** → **Open**
   - This bypasses macOS Gatekeeper for unsigned apps

4. Run the application - the Welsh model will download automatically (~47MB)

**Troubleshooting**: If you get "app is damaged", run in Terminal:
```bash
xattr -cr /path/to/VoskWelshSpeechRecognition
```

### Windows

**Requirements**: Windows 10 or later (64-bit)

1. Download [VoskWelshSpeechRecognition-Windows-x64-v1.0.0.zip](../../releases)

2. Extract the zip file to a folder (e.g., `C:\VoskWelsh\`)

3. Double-click `VoskWelshSpeechRecognition.exe`

4. **First time only**: If Windows SmartScreen appears, click **"More info"** → **"Run anyway"**

5. The Welsh model will download automatically (~47MB)

### Linux

**Requirements**: Any modern 64-bit Linux distribution

1. Download [VoskWelshSpeechRecognition-Linux-x64-v1.0.0.tar.gz](../../releases)

2. Extract:
   ```bash
   tar -xzf VoskWelshSpeechRecognition-Linux-x64-v1.0.0.tar.gz
   cd VoskWelshSpeechRecognition-Linux-x64-v1.0.0
   ```

3. Install OpenAL (audio library):
   ```bash
   # Ubuntu/Debian
   sudo apt-get install libopenal1

   # Fedora/RHEL
   sudo dnf install openal-soft

   # Arch
   sudo pacman -S openal
   ```

4. Make executable and run:
   ```bash
   chmod +x VoskWelshSpeechRecognition
   ./VoskWelshSpeechRecognition
   ```

5. The Welsh model will download automatically (~47MB)

## How to Use

1. **Launch the application**

2. **Grant microphone permissions** when prompted

3. **Click "Start Recording / Dechrau Recordio"**

4. **Speak in Welsh** - You'll see partial transcriptions appear in the yellow "Live Input" box

5. **Pause speaking** - After a brief silence, the completed sentence moves to the "Completed Transcriptions" box with a timestamp

6. **Click "Stop Recording / Stopio Recordio"** when finished

7. **Click "Clear / Clirio"** to reset the transcriptions

## Technical Details

### Built With

- [.NET 8.0](https://dotnet.microsoft.com/) - Cross-platform framework
- [Avalonia UI](https://avaloniaui.net/) - Cross-platform XAML-based UI framework
- [Vosk](https://alphacephei.com/vosk/) - Offline speech recognition engine
- [OpenTK.OpenAL](https://opentk.net/) - Cross-platform audio capture
- [Kaldi Welsh Models](https://huggingface.co/techiaith/kaldi-cy) - Welsh language models from Bangor University

### Model Details

The application automatically downloads the Welsh speech recognition model from:
- **Source**: https://huggingface.co/techiaith/kaldi-cy
- **Size**: ~47 MB
- **Location**: Stored in `Models/vosk-model-cy/` in the application directory
- **Quality**: Trained on Welsh broadcast media and conversational speech

### Privacy

- ✅ **100% Offline** - All speech recognition happens locally on your device
- ✅ **No data collection** - Nothing is sent to external servers (except the one-time model download)
- ✅ **No internet required** - After the model downloads, the app works completely offline

## Building from Source

See [DISTRIBUTION.md](DISTRIBUTION.md) for detailed build instructions.

Quick build:
```bash
# Clone the repository
git clone https://github.com/yourusername/vosk-windows.git
cd vosk-windows

# Build for all platforms
./build-all.sh

# Or build for your platform only
./build-macos.sh     # macOS
./build-windows.sh   # Windows
./build-linux.sh     # Linux

# Or build/run from source
cd src
dotnet build
dotnet run
```

## Credits

- **Vosk**: https://alphacephei.com/vosk/
- **Welsh Language Models**: [Bangor University - techiaith](https://github.com/techiaith)
- **Original CLI inspiration**: [vosk-tui](https://github.com/Cymru-Breizh-Agile-Cymru-Project/vosk-tui)

## License

MIT License - see [LICENSE](LICENSE) file for details

## Support

For issues, questions, or contributions, please open an issue on the [GitHub repository](../../issues).

---

**Diolch am ddefnyddio'r ap hwn! / Thank you for using this app!** 🏴󠁧󠁢󠁷󠁬󠁳󠁿
