# Ap adnabod Lleferydd Cymraeg Byw / Real-time Welsh Speech Recognition App

**[English version below](#english) | [Fersiwn Saesneg isod](#english)**

---

## 🏴󠁧󠁢󠁷󠁬󠁳󠁿 Cymraeg

Rhaglen bwrdd gwaith trawsplatfform (Windows, Mac a Linux) ar gyfer adnabod lleferydd Cymraeg byw gan ddefnyddio model Vosk.

![Llwyfan](https://img.shields.io/badge/llwyfan-macOS%20%7C%20Windows%20%7C%20Linux-blue)
![.NET](https://img.shields.io/badge/.NET-8.0-purple)

### ✨ Nodweddion

- 🎤 **Adnabod Lleferydd Amser Real** - Gweld trawsgrifiadau wrth i chi siarad
- 🏴󠁧󠁢󠁷󠁬󠁳󠁿 **Cefnogaeth Iaith Gymraeg** - Yn defnyddio modelau Kaldi o Brifysgol Bangor
- 📴 **100% All-lein** - Dim angen cysylltiad i'r rhyngrwyd ar ôl y tro cyntaf
- 🖥️ **Trawsplatform** - macOS, Windows, a Linux
- 🔄 **Lawrlwytho Modelau'n Awtomatig** - Mae'r model Cymraeg yn lawrlwytho'n awtomatig (~47MB)
- 🌐 **Rhyngwyneb Dwyieithog** - Rhyngwyneb yn Gymraeg a Saesneg

### 📥 Ar gyfer Defnyddwyr

Gweler **[USER_README.md](USER_README.md)** am ddolenni lawrlwytho a chyfarwyddiadau gosod.

### 🚀 Ar gyfer Datblygwyr

#### Cychwyn Cyflym

```bash
# Clonio'r ystorfa
git clone https://github.com/yourusername/vosk-windows.git
cd vosk-windows

# Adeiladu a rhedeg o'r ffynhonnell
cd src
dotnet restore
dotnet build
dotnet run
```

#### Adeiladu ar gyfer dosbarthu

```bash
# Adeiladu ar gyfer eich llwyfan cyfredol
./build-macos.sh     # macOS
./build-windows.sh   # Windows (o Git Bash neu WSL)
./build-linux.sh     # Linux

# Neu adeiladu ar gyfer pob llwyfan ar unwaith
./build-all.sh

# Creu archifau dosbarthu
./create-releases.sh
```

Gweler **[DISTRIBUTION.md](DISTRIBUTION.md)** am gyfarwyddiadau adeiladu a dosbarthu manwl.

### 📁 Strwythur y Prosiect

```
vosk-windows/
├── src/                      # Côd ffynhonnell
│   ├── MainWindow.axaml      # Cynllun y rhyngwyneb defnyddiwr
│   ├── MainWindow.axaml.cs   # Cod yr ap
│   ├── Assets/               # Eiconau
│   └── *.csproj              # Ffeil prosiect
├── build-*.sh                # Sgriptiau adeiladu ar gyfer pob llwyfan
├── dist/                     # Allbwn adeiladu (gitignored)
├── DISTRIBUTION.md           # Canllaw adeiladu/defnyddio
└── USER_README.md            # Dogfennaeth defnyddiwr terfynol
```

### 🛠️ Technolegau a ddefnyddir

- [.NET 8.0](https://dotnet.microsoft.com/) - Fframwaith draws-lwyfan
- [Avalonia UI](https://avaloniaui.net/) - Rhyngwyneb defnyddiwr XAML draws-lwyfan
- [Vosk](https://alphacephei.com/vosk/) - Adnabod lleferydd all-lein
- [OpenTK.OpenAL](https://opentk.net/) - Dal sain trawsplatfform
- [Modelau Cymraeg Kaldi](https://huggingface.co/techiaith/kaldi-cy) - gan Gweltaz Duval-Gwennoc, Preben Vanberg, Sasha Wanasky a techiaith Prifysgol Bangor

### 📚 Dogfennaeth

- **[USER_README.md](USER_README.md)** - Ar gyfer defnyddwyr terfynol (lawrlwytho a gosod)
- **[DISTRIBUTION.md](DISTRIBUTION.md)** - Ar gyfer datblygwyr (adeiladu a phecynnu)
- **[ADDING_ICONS.md](ADDING_ICONS.md)** - Sut i addasu eicon yr ap
- **[ICON_QUICK_SETUP.md](ICON_QUICK_SETUP.md)** - Canllaw gosod eicon cyflym
- **[VOSK_ON_WINDOWS.md](VOSK_ON_WINDOWS.md)** - Nodiadau technegol ar integreiddio Vosk

### 🔗 Adnoddau

#### Modelau Iaith Gymraeg
- [techiaith/kaldi-cy](https://huggingface.co/techiaith/kaldi-cy) - Hugging Face
- [vosk-cymraeg](https://github.com/Cymru-Breizh-Agile-Cymru-Project/vosk-cymraeg) - GitHub

#### Dogfennaeth Vosk
- [Gwefan Swyddogol](https://alphacephei.com/vosk/)
- [Ystorfa GitHub](https://github.com/alphacep/vosk-api)
- [Modelau ar Gael](https://alphacephei.com/vosk/models)

#### Prosiectau Cysylltiedig
- [vosk-tui](https://github.com/Cymru-Breizh-Agile-Cymru-Project/vosk-tui) - Rhyngwyneb Terfynell (Python)
- [Techiaith](https://techiaith.cymru/) - Technoleg iaith Gymraeg

### 🙏 Cydnabyddiaeth

- **Vosk** - Alpha Cephei Inc. (trwydded Apache 2.0)
- **Modelau Iaith Gymraeg** - Prifysgol Bangor / Techiaith
- **Ysbrydoliaeth** - gweithrediad vosk-tui Python
- **Eicon** - Dyluniad arfer gan ddefnyddio lliwiau Baner Cymru

### 📄 Trwydded

Mae côd yr ap hwn yn cael ei ddarparu at ddibenion addysgol. Adolygwch drwyddedau cydrannau unigol:

- **Côd yr Ap** - Trwydded MIT (gweler [LICENSE](LICENSE))
- **Vosk** - Apache 2.0
- **Avalonia UI** - MIT
- **Modelau Cymraeg** - Gwiriwch drwyddedau model penodol yn y ffynhonnell

### 🤝 Cyfrannu

Croeso i chi gysylltu drwy ein hysbysu am gwallau ac/neu i gynnig gwelliannau.


**Pob lwc!** 🏴󠁧󠁢󠁷󠁬󠁳󠁿


---

<a name="english"></a>

## 🇬🇧 English

**[Back to Welsh / Yn ôl i'r Gymraeg](#-cymraeg)**

A cross-platform desktop application for real-time Welsh speech recognition using Vosk offline speech recognition.

![Platform](https://img.shields.io/badge/platform-macOS%20%7C%20Windows%20%7C%20Linux-blue)
![.NET](https://img.shields.io/badge/.NET-8.0-purple)

### ✨ Features

- 🎤 **Real-time Speech Recognition** - See transcriptions as you speak
- 🏴󠁧󠁢󠁷󠁬󠁳󠁿 **Welsh Language Support** - Uses Kaldi models from Bangor University
- 📴 **100% Offline** - No internet required after first run
- 🖥️ **Cross-Platform** - macOS, Windows, and Linux
- 🔄 **Auto-Download Models** - Welsh model downloads automatically (~47MB)
- 🌐 **Bilingual UI** - Interface in Welsh and English

### 📥 For End Users

See **[USER_README.md](USER_README.md)** for download links and installation instructions.

### 🚀 For Developers

#### Quick Start

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

#### Build for Distribution

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

### 📁 Project Structure

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

### 🛠️ Tech Stack

- [.NET 8.0](https://dotnet.microsoft.com/) - Cross-platform framework
- [Avalonia UI](https://avaloniaui.net/) - Cross-platform XAML UI
- [Vosk](https://alphacephei.com/vosk/) - Offline speech recognition
- [OpenTK.OpenAL](https://opentk.net/) - Cross-platform audio capture
- [Kaldi Welsh Models](https://huggingface.co/techiaith/kaldi-cy) - by Gweltaz Duval-Gwennoc, Preben Vanberg, Sasha Wanasky and techiaith Bangor University

### 📚 Documentation

- **[USER_README.md](USER_README.md)** - For end users (download & install)
- **[DISTRIBUTION.md](DISTRIBUTION.md)** - For developers (building & packaging)
- **[ADDING_ICONS.md](ADDING_ICONS.md)** - How to customize the app icon
- **[ICON_QUICK_SETUP.md](ICON_QUICK_SETUP.md)** - Quick icon setup guide
- **[VOSK_ON_WINDOWS.md](VOSK_ON_WINDOWS.md)** - Technical notes on Vosk integration

### 🔗 Resources

#### Welsh Language Models
- [techiaith/kaldi-cy](https://huggingface.co/techiaith/kaldi-cy) - Hugging Face
- [vosk-cymraeg](https://github.com/Cymru-Breizh-Agile-Cymru-Project/vosk-cymraeg) - GitHub

#### Vosk Documentation
- [Official Website](https://alphacephei.com/vosk/)
- [GitHub Repository](https://github.com/alphacep/vosk-api)
- [Available Models](https://alphacephei.com/vosk/models)

#### Related Projects
- [vosk-tui](https://github.com/Cymru-Breizh-Agile-Cymru-Project/vosk-tui) - Terminal UI (Python)
- [Techiaith](https://techiaith.cymru/) - Welsh language technology

### 🙏 Credits

- **Vosk** - Alpha Cephei Inc. (Apache 2.0 license)
- **Welsh Language Models** - Bangor University / Techiaith
- **Inspiration** - vosk-tui Python implementation
- **Icon** - Custom design using Welsh flag colors

### 📄 License

This application code is provided for educational purposes. Please review individual component licenses:

- **Application Code** - MIT License (see [LICENSE](LICENSE))
- **Vosk** - Apache 2.0
- **Avalonia UI** - MIT
- **Welsh Models** - Check specific model licenses at source

### 🤝 Contributing

Contributions welcome! Please feel free to submit issues and pull requests.

---

**Good luck!** 🏴󠁧󠁢󠁷󠁬󠁳󠁿
