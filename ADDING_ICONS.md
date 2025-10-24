# Adding Application Icons

This guide explains how to add custom icons to the Vosk Welsh Speech Recognition app.

## Current Icon

A basic SVG icon has been created at `src/Assets/icon.svg` with:
- Welsh flag colors (green and red)
- Microphone symbol
- Sound waves

You can replace this with your own design!

## Converting SVG to Platform-Specific Formats

### Option 1: Online Converters (Easiest)

1. Go to https://cloudconvert.com/svg-to-icns (for macOS)
2. Upload `src/Assets/icon.svg`
3. Convert and download as `icon.icns`
4. Save to `src/Assets/icon.icns`

Repeat for Windows:
1. Go to https://cloudconvert.com/svg-to-ico
2. Upload `src/Assets/icon.svg`
3. Set output to include 256x256, 128x128, 64x64, 48x48, 32x32, 16x16
4. Download as `icon.ico`
5. Save to `src/Assets/icon.ico`

For Linux:
1. Go to https://cloudconvert.com/svg-to-png
2. Upload `src/Assets/icon.svg`
3. Set dimensions to 512x512
4. Download as `icon.png`
5. Save to `src/Assets/icon.png`

### Option 2: Using ImageMagick/Inkscape (Command Line)

Install ImageMagick:
```bash
# macOS
brew install imagemagick

# Ubuntu/Debian
sudo apt-get install imagemagick

# Fedora
sudo dnf install imagemagick
```

Convert:
```bash
cd src/Assets

# Generate PNG at various sizes
magick icon.svg -resize 512x512 icon-512.png
magick icon.svg -resize 256x256 icon-256.png
magick icon.svg -resize 128x128 icon-128.png
magick icon.svg -resize 64x64 icon-64.png
magick icon.svg -resize 32x32 icon-32.png
magick icon.svg -resize 16x16 icon-16.png

# Create .ico for Windows (multiple sizes)
magick icon-16.png icon-32.png icon-64.png icon-128.png icon-256.png icon.ico

# For macOS .icns, use iconutil (macOS only):
mkdir icon.iconset
magick icon.svg -resize 16x16 icon.iconset/icon_16x16.png
magick icon.svg -resize 32x32 icon.iconset/icon_16x16@2x.png
magick icon.svg -resize 32x32 icon.iconset/icon_32x32.png
magick icon.svg -resize 64x64 icon.iconset/icon_32x32@2x.png
magick icon.svg -resize 128x128 icon.iconset/icon_128x128.png
magick icon.svg -resize 256x256 icon.iconset/icon_128x128@2x.png
magick icon.svg -resize 256x256 icon.iconset/icon_256x256.png
magick icon.svg -resize 512x512 icon.iconset/icon_256x256@2x.png
magick icon.svg -resize 512x512 icon.iconset/icon_512x512.png
magick icon.svg -resize 1024x1024 icon.iconset/icon_512x512@2x.png
iconutil -c icns icon.iconset -o icon.icns
rm -rf icon.iconset
```

## Configuring the Project

Once you have the icon files (`icon.icns`, `icon.ico`, `icon.png`), update the project file:

Edit `src/VoskWelshSpeechRecognition.csproj` and add:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <BuiltInComInteropSupport>true</BuiltInComInteropSupport>
    <ApplicationManifest>app.manifest</ApplicationManifest>
    <AvaloniaUseCompiledBindingsByDefault>true</AvaloniaUseCompiledBindingsByDefault>

    <!-- Application Icon -->
    <ApplicationIcon>Assets/icon.ico</ApplicationIcon>
  </PropertyGroup>

  <!-- Include icon files in build -->
  <ItemGroup>
    <!-- Windows icon -->
    <None Include="Assets/icon.ico" />

    <!-- macOS icon (for app bundle) -->
    <None Include="Assets/icon.icns" Condition="'$(RuntimeIdentifier)' == 'osx-arm64' or '$(RuntimeIdentifier)' == 'osx-x64'">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>

    <!-- Linux icon -->
    <None Include="Assets/icon.png" Condition="'$(RuntimeIdentifier)' == 'linux-x64'">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
  </ItemGroup>

  <!-- Existing PackageReference items below... -->
</Project>
```

## For macOS App Bundle (Advanced)

To create a proper `.app` bundle with icon on macOS:

1. After building, create app bundle structure:
```bash
./build-macos.sh
cd dist/macos-osx-arm64

mkdir -p VoskWelshSpeechRecognition.app/Contents/{MacOS,Resources}
mv VoskWelshSpeechRecognition VoskWelshSpeechRecognition.app/Contents/MacOS/
mv *.dylib VoskWelshSpeechRecognition.app/Contents/MacOS/
cp ../../src/Assets/icon.icns VoskWelshSpeechRecognition.app/Contents/Resources/

cat > VoskWelshSpeechRecognition.app/Contents/Info.plist << 'EOF'
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>CFBundleExecutable</key>
    <string>VoskWelshSpeechRecognition</string>
    <key>CFBundleIconFile</key>
    <string>icon</string>
    <key>CFBundleIdentifier</key>
    <string>com.techiaith.voskwelsh</string>
    <key>CFBundleName</key>
    <string>Vosk Welsh Speech Recognition</string>
    <key>CFBundlePackageType</key>
    <string>APPL</string>
    <key>CFBundleShortVersionString</key>
    <string>1.0.0</string>
    <key>LSMinimumSystemVersion</key>
    <string>10.15</string>
</dict>
</plist>
EOF
```

2. Now you can double-click the `.app` to launch!

## Quick Test

After adding icons and rebuilding:

**Windows**: The `.exe` file should show your icon in File Explorer
**macOS**: The app bundle should show your icon in Finder
**Linux**: The `.png` icon will be used by the desktop environment

## Verification

Build and check:
```bash
./build-all.sh

# Check Windows executable has icon
file dist/windows-x64/VoskWelshSpeechRecognition.exe

# Check macOS has icon file
ls -la dist/macos-*/icon.icns

# Check Linux has icon
ls -la dist/linux-x64/icon.png
```

---

**Tip**: For best results, design your icon at 1024x1024 pixels, then scale down to smaller sizes.
