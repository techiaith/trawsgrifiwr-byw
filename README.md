# Ap adnabod Lleferydd Cymraeg Byw / Real-time Welsh Speech Recognition App

**[English version below](#english) | [Fersiwn Saesneg isod](#english)**

---

## Cymraeg

Rhaglen bwrdd gwaith trawsplatfform (Windows, Mac a Linux) ar gyfer adnabod lleferydd Cymraeg byw gan ddefnyddio model Vosk.

![Llwyfan](https://img.shields.io/badge/llwyfan-macOS%20%7C%20Windows%20%7C%20Linux-blue)
![.NET](https://img.shields.io/badge/.NET-8.0-purple)

Mae'r Trawsgrifiwr Byw angen 'OpenAL' wedi ei osod hefyd os am ei ddefnyddio ar beiriant Windows [OpenAL 1.1 Windows Installer](https://www.openal.org/downloads/oalinst.zip)

### 📥 Ar gyfer Defnyddwyr

#### Gosod

**Lawrlwythwch y rhyddhad diweddaraf o [GitHub Releases](https://github.com/techiaith/trawsgrifiwr-byw/releases)**

##### macOS
- Lawrlwythwch `Trawsgrifiwr-Byw-macOS-*.pkg` (hawsaf - pecyn gosodwr)
- Neu lawrlwythwch `Trawsgrifiwr-Byw-macOS-*.zip` (gosodiad â llaw)

**Pwysig:** Bydd macOS yn dangos rhybudd diogelwch ar gyfer rhaglenni heb eu llofnodi. I osod:
1. **Clic-dde** (neu Ctrl+clic) ar y ffeil `.pkg` a dewis **"Agor"**
2. Cliciwch **"Agor"** yn y ddeialog diogelwch
3. Neu rhedeg yn y Terfynell: `sudo installer -pkg /llwybr/i/ffeil.pkg -target /`

##### Windows
- Lawrlwythwch a rhedeg `Trawsgrifiwr-Byw-v*.msi` (gosodwr)
- Neu lawrlwythwch a datgywasgu `Trawsgrifiwr-Byw-Windows-x64-*.zip`
- **Pwysig:** Rhaid gosod [OpenAL 1.1](https://www.openal.org/downloads/oalinst.zip) yn gyntaf

##### Linux
- Lawrlwythwch a datgywasgu `Trawsgrifiwr-Byw-Linux-x64-*.tar.gz`
- Gwnewch yn weithredadwy: `chmod +x Trawsgrifiwr-Byw`
- Rhedeg: `./Trawsgrifiwr-Byw`

### ✨ Nodweddion

- 🎤 **Adnabod Lleferydd Amser Real** - Gweld trawsgrifiadau wrth i chi siarad
- 🏴󠁧󠁢󠁷󠁬󠁳󠁿 **Cefnogaeth Iaith Gymraeg** - Yn defnyddio modelau Kaldi o Brifysgol Bangor
- 📴 **100% All-lein** - Dim angen cysylltiad i'r rhyngrwyd ar ôl y tro cyntaf
- 🖥️ **Trawsplatform** - macOS, Windows, a Linux
- 🔄 **Lawrlwytho Modelau'n Awtomatig** - Mae'r model Cymraeg yn lawrlwytho'n awtomatig (~47MB)
- 🌐 **Rhyngwyneb Dwyieithog** - Rhyngwyneb yn Gymraeg a Saesneg

### 🚀 Ar gyfer Datblygwyr

#### Cychwyn Cyflym

```bash
# Clonio'r ystorfa
git clone https://github.com/techiaith/trawsgrifiwr-byw.git
cd trawsgrifiwr-byw

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

### 📁 Strwythur y Prosiect

```
trawsgrifiwr-byw/
├── src/                      # Côd ffynhonnell
│   ├── MainWindow.axaml      # Cynllun y rhyngwyneb defnyddiwr
│   ├── MainWindow.axaml.cs   # Cod yr ap
│   ├── Assets/               # Eiconau
│   └── *.csproj              # Ffeil prosiect
├── build-*.sh                # Sgriptiau adeiladu ar gyfer pob llwyfan
└── dist/                     # Allbwn adeiladu (gitignored)
```

### 🛠️ Technolegau a ddefnyddir

- [.NET 8.0](https://dotnet.microsoft.com/) - Fframwaith draws-lwyfan
- [Avalonia UI](https://avaloniaui.net/) - Rhyngwyneb defnyddiwr XAML draws-lwyfan
- [Vosk](https://alphacephei.com/vosk/) - Adnabod lleferydd all-lein
- [OpenTK.OpenAL](https://opentk.net/) - Dal sain trawsplatfform
- [Modelau Cymraeg Kaldi](https://huggingface.co/techiaith/kaldi-cy) - gan Gweltaz Duval-Gwennoc, Preben Vanberg, Sasha Wanasky a techiaith Prifysgol Bangor

Mae'r Trawsgrifiwr Byw angen 'OpenAL' wedi ei osod hefyd os am ei ddefnyddio ar beiriant Windows [OpenAL 1.1 Windows Installer](https://www.openal.org/downloads/oalinst.zip)

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

- **Kaldi Cymraeg** - Gweltaz Duval-Guennoc (@gweltou), Preben Vanberg (@prvInSpace), Sasha Wanasky (@wanasash), Uned Technolegau Iaith, Prifysgol Bangor (@techiaith). Arianwyd gan Agile Cymru, Llywodraeth Cymru. (https://github.com/Cymru-Breizh-Agile-Cymru-Project/vosk-cymraeg)
- **Vosk** - Alpha Cephei Inc. (trwydded Apache 2.0)
- **Modelau Iaith Gymraeg** - Prifysgol Bangor / Techiaith
- **Ysbrydoliaeth** - gweithrediad vosk-tui Python gan Preben Vanberg (https://github.com/Cymru-Breizh-Agile-Cymru-Project/vosk-tui)

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

## English

**[Back to Welsh / Yn ôl i'r Gymraeg](#-cymraeg)**

A cross-platform desktop application for real-time Welsh speech recognition using Vosk offline speech recognition.

![Platform](https://img.shields.io/badge/platform-macOS%20%7C%20Windows%20%7C%20Linux-blue)
![.NET](https://img.shields.io/badge/.NET-8.0-purple)

The Trawsgrifiwr Byw also needs 'OpenAL' installed if it is to be used on a Windows machine [OpenAL 1.1 Windows Installer](https://www.openal.org/downloads/oalinst.zip)

### ✨ Features

- 🎤 **Real-time Speech Recognition** - See transcriptions as you speak
- 🏴󠁧󠁢󠁷󠁬󠁳󠁿 **Welsh Language Support** - Uses Kaldi models from Bangor University
- 📴 **100% Offline** - No internet required after first run
- 🖥️ **Cross-Platform** - macOS, Windows, and Linux
- 🔄 **Auto-Download Models** - Welsh model downloads automatically (~47MB)
- 🌐 **Bilingual UI** - Interface in Welsh and English

### 📥 For End Users

#### Installation

**Download the latest release from [GitHub Releases](https://github.com/techiaith/trawsgrifiwr-byw/releases)**

##### macOS
- Download `Trawsgrifiwr-Byw-macOS-*.pkg` (easiest - installer package)
- Or download `Trawsgrifiwr-Byw-macOS-*.zip` (manual installation)

**Important:** macOS will show a security warning for unsigned applications. To install:
1. **Right-click** (or Ctrl+click) the `.pkg` file and select **"Open"**
2. Click **"Open"** in the security dialog
3. Or run in Terminal: `sudo installer -pkg /path/to/file.pkg -target /`

##### Windows
- Download and run `Trawsgrifiwr-Byw-v*.msi` (installer)
- Or download and extract `Trawsgrifiwr-Byw-Windows-x64-*.zip`
- **Important:** You must install [OpenAL 1.1](https://www.openal.org/downloads/oalinst.zip) first

##### Linux
- Download and extract `Trawsgrifiwr-Byw-Linux-x64-*.tar.gz`
- Make executable: `chmod +x Trawsgrifiwr-Byw`
- Run: `./Trawsgrifiwr-Byw`

See **[USER_README.md](USER_README.md)** for detailed installation instructions.

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
trawsgrifiwr-byw/
├── src/                      # Source code
│   ├── MainWindow.axaml      # UI layout
│   ├── MainWindow.axaml.cs   # Application logic
│   ├── Assets/               # Icons
│   └── *.csproj              # Project file
├── build-*.sh                # Build scripts for each platform
└──dist/                     # Build output (gitignored)
```

### 🛠️ Tech Stack

- [.NET 8.0](https://dotnet.microsoft.com/) - Cross-platform framework
- [Avalonia UI](https://avaloniaui.net/) - Cross-platform XAML UI
- [Vosk](https://alphacephei.com/vosk/) - Offline speech recognition
- [OpenTK.OpenAL](https://opentk.net/) - Cross-platform audio capture
- [Kaldi Welsh Models](https://huggingface.co/techiaith/kaldi-cy) - by Gweltaz Duval-Gwennoc, Preben Vanberg, Sasha Wanasky and techiaith Bangor University

The Trawsgrifiwr Byw also needs 'OpenAL' installed if it is to be used on a Windows machine [OpenAL 1.1 Windows Installer](https://www.openal.org/downloads/oalinst.zip)

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
