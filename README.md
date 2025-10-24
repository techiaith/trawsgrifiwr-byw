# Vosk Welsh Speech Recognition / Adnabod Lleferydd Cymraeg

A cross-platform desktop application for real-time Welsh speech recognition using Vosk offline speech recognition.

![Platform](https://img.shields.io/badge/platform-macOS%20%7C%20Windows%20%7C%20Linux-blue)
![.NET](https://img.shields.io/badge/.NET-8.0-purple)

## ✨ Features

- 🎤 **Real-time Speech Recognition** - See transcriptions as you speak
- 🏴󠁧󠁢󠁷󠁬󠁳󠁿 **Welsh Language Support** - Uses Kaldi models from Bangor University
- 📴 **100% Offline** - No internet required after first run
- 🖥️ **Cross-Platform** - macOS, Windows, and Linux
- 🔄 **Auto-Download Models** - Welsh model downloads automatically (~47MB)
- 🌐 **Bilingual UI** - Interface in Welsh and English

## 📥 For End Users

See **[USER_README.md](USER_README.md)** for download links and installation instructions.

## 🚀 For Developers

### Quick Start

```bash
# Clone the repository
git clone https://github.com/yourusername/vosk-windows.git
cd vosk-windows

# Build and run from source
cd src
dotnet restore
dotnet build
dotnet run
```

### Build for Distribution

```bash
# Build for your current platform
./build-macos.sh     # macOS
./build-windows.sh   # Windows (from Git Bash or WSL)
./build-linux.sh     # Linux

# Or build for all platforms at once
./build-all.sh

# Create distribution archives
./create-releases.sh
```

See **[DISTRIBUTION.md](DISTRIBUTION.md)** for detailed build and distribution instructions.

## 📁 Project Structure

```
vosk-windows/
├── src/                      # Source code
│   ├── MainWindow.axaml      # UI layout
│   ├── MainWindow.axaml.cs   # Application logic
│   ├── Assets/               # Icons
│   └── *.csproj              # Project file
├── build-*.sh                # Build scripts for each platform
├── dist/                     # Build output (gitignored)
├── DISTRIBUTION.md           # Build/deployment guide
└── USER_README.md            # End-user documentation
```

## 🛠️ Tech Stack

- [.NET 8.0](https://dotnet.microsoft.com/) - Cross-platform framework
- [Avalonia UI](https://avaloniaui.net/) - Cross-platform XAML UI
- [Vosk](https://alphacephei.com/vosk/) - Offline speech recognition
- [OpenTK.OpenAL](https://opentk.net/) - Cross-platform audio capture
- [Kaldi Welsh Models](https://huggingface.co/techiaith/kaldi-cy) - From Bangor University

## 📚 Documentation

- **[USER_README.md](USER_README.md)** - For end users (download & install)
- **[DISTRIBUTION.md](DISTRIBUTION.md)** - For developers (building & packaging)
- **[ADDING_ICONS.md](ADDING_ICONS.md)** - How to customize the app icon
- **[ICON_QUICK_SETUP.md](ICON_QUICK_SETUP.md)** - Quick icon setup guide
- **[VOSK_ON_WINDOWS.md](VOSK_ON_WINDOWS.md)** - Technical notes on Vosk integration

## 🔗 Resources

### Welsh Language Models
- [techiaith/kaldi-cy](https://huggingface.co/techiaith/kaldi-cy) - Hugging Face
- [vosk-cymraeg](https://github.com/Cymru-Breizh-Agile-Cymru-Project/vosk-cymraeg) - GitHub

### Vosk Documentation
- [Official Website](https://alphacephei.com/vosk/)
- [GitHub Repository](https://github.com/alphacep/vosk-api)
- [Available Models](https://alphacephei.com/vosk/models)

### Related Projects
- [vosk-tui](https://github.com/Cymru-Breizh-Agile-Cymru-Project/vosk-tui) - Terminal UI (Python)
- [Techiaith](https://techiaith.cymru/) - Welsh language technology

## 🙏 Credits

- **Vosk** - Alpha Cephei Inc. (Apache 2.0 license)
- **Welsh Language Models** - Bangor University / Techiaith
- **Inspiration** - vosk-tui Python implementation
- **Icon** - Custom design using Welsh flag colors

## 📄 License

This application code is provided for educational purposes. Please review individual component licenses:

- **Application Code** - MIT License (see [LICENSE](LICENSE))
- **Vosk** - Apache 2.0
- **Avalonia UI** - MIT
- **Welsh Models** - Check specific model licenses at source

## 🤝 Contributing

Contributions welcome! Please feel free to submit issues and pull requests.

---

**Pob lwc! / Good luck!** 🏴󠁧󠁢󠁷󠁬󠁳󠁿
