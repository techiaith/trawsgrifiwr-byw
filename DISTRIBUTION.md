# Vosk Welsh Speech Recognition - Distribution Guide

## Building for Distribution

This guide explains how to build self-contained executables for macOS, Windows, and Linux.

### Prerequisites

- .NET 8.0 SDK installed
- Bash shell (macOS/Linux have this by default, Windows users can use Git Bash or WSL)

---

## Quick Start

### Build for your current platform only:

```bash
# macOS
./build-macos.sh

# Windows (from Git Bash or WSL)
./build-windows.sh

# Linux
./build-linux.sh
```

### Build for all platforms at once:

```bash
./build-all.sh
```

### Create distribution archives (zip/tar.gz):

```bash
./create-releases.sh
```

---

## Output Structure

After building, you'll find:

```
dist/
├── macos-osx-arm64/          # macOS Apple Silicon build
│   └── VoskWelshSpeechRecognition
├── macos-osx-x64/            # macOS Intel build
│   └── VoskWelshSpeechRecognition
├── windows-x64/              # Windows build
│   └── VoskWelshSpeechRecognition.exe
└── linux-x64/                # Linux build
    └── VoskWelshSpeechRecognition

releases/                      # Created by create-releases.sh
├── VoskWelshSpeechRecognition-macOS-osx-arm64-v1.0.0.zip
├── VoskWelshSpeechRecognition-Windows-x64-v1.0.0.zip
└── VoskWelshSpeechRecognition-Linux-x64-v1.0.0.tar.gz
```

---

## File Sizes (Approximate)

- **macOS**: ~60-80 MB
- **Windows**: ~50-70 MB
- **Linux**: ~60-80 MB

These are self-contained single files that include:
- The application
- .NET 8 runtime
- All dependencies (Vosk, OpenAL, Avalonia, etc.)

---

## Testing Builds

### macOS:
```bash
cd dist/macos-osx-arm64/
./VoskWelshSpeechRecognition
```

### Windows:
1. Copy the `dist/windows-x64/` folder to a Windows machine
2. Double-click `VoskWelshSpeechRecognition.exe`

### Linux:
```bash
cd dist/linux-x64/
./VoskWelshSpeechRecognition
```

**Note**: Linux users may need OpenAL installed:
```bash
# Ubuntu/Debian
sudo apt-get install libopenal1

# Fedora/RHEL
sudo dnf install openal-soft

# Arch
sudo pacman -S openal
```

---

## Distributing to End Users

### Method 1: GitHub Releases (Recommended)

1. Create a GitHub repository
2. Run `./build-all.sh && ./create-releases.sh`
3. Create a new release on GitHub
4. Upload the files from `releases/` folder
5. Users download the appropriate file for their platform

### Method 2: Direct Download

Host the release archives on your web server and provide download links.

### Method 3: Package Managers

- **macOS**: Create a Homebrew formula
- **Windows**: Create a Chocolatey package
- **Linux**: Create .deb or .rpm packages

---

## User Installation Instructions

### macOS:

1. Download `VoskWelshSpeechRecognition-macOS-osx-arm64-v1.0.0.zip` (for M1/M2/M3) or `osx-x64` (for Intel)
2. Extract the zip file
3. Right-click `VoskWelshSpeechRecognition` → Open (first time only, to bypass Gatekeeper)
4. Click "Open" when prompted
5. The app will download the Welsh language model (~47MB) on first run

### Windows:

1. Download `VoskWelshSpeechRecognition-Windows-x64-v1.0.0.zip`
2. Extract the zip file
3. Double-click `VoskWelshSpeechRecognition.exe`
4. If Windows SmartScreen appears, click "More info" → "Run anyway"
5. The app will download the Welsh language model (~47MB) on first run

### Linux:

1. Download `VoskWelshSpeechRecognition-Linux-x64-v1.0.0.tar.gz`
2. Extract: `tar -xzf VoskWelshSpeechRecognition-Linux-x64-v1.0.0.tar.gz`
3. Install OpenAL if needed: `sudo apt-get install libopenal1`
4. Make executable: `chmod +x VoskWelshSpeechRecognition`
5. Run: `./VoskWelshSpeechRecognition`
6. The app will download the Welsh language model (~47MB) on first run

---

## Advanced: Reducing File Size

If you want smaller executables (at the cost of requiring .NET to be installed):

```bash
# Framework-dependent (requires .NET 8 installed on user's machine)
dotnet publish VoskWelshSpeechRecognitionAvalonia \
    --configuration Release \
    --output dist/framework-dependent \
    -p:PublishSingleFile=true

# Size: ~5-10 MB instead of ~60 MB
# Tradeoff: Users must install .NET 8 Runtime first
```

---

## Troubleshooting

### macOS: "App is damaged and can't be opened"

This is Gatekeeper blocking unsigned apps. Fix:
```bash
xattr -cr VoskWelshSpeechRecognition
```

Or right-click → Open → Open

### Windows: "Windows protected your PC"

Click "More info" → "Run anyway"

To avoid this, you'd need to code-sign the executable (requires a certificate ~$100/year)

### Linux: "error while loading shared libraries: libopenal.so"

Install OpenAL:
```bash
sudo apt-get install libopenal1
```

---

## Version Updates

To create a new release version:

1. Update version in `create-releases.sh` (line 10):
   ```bash
   VERSION="1.1.0"  # Change this
   ```

2. Rebuild everything:
   ```bash
   ./build-all.sh
   ./create-releases.sh
   ```

3. Upload new archives to GitHub Releases or your distribution platform

---

## Summary

✅ **Single command**: `./build-all.sh` creates executables for all platforms
✅ **Self-contained**: No dependencies needed (except OpenAL on Linux)
✅ **Small-ish**: ~60MB per platform
✅ **Easy for users**: Download, extract, run
✅ **Model auto-downloads**: Welsh model fetches automatically on first run

The .NET approach makes this **significantly easier** than distributing a Python app!
