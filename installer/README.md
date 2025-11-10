# Windows MSI Installer

This directory contains the WiX Toolset configuration for creating a Windows MSI installer for Trawsgrifiwr Byw.

## Requirements

- **Windows OS** (the MSI build process requires Windows)
- **WiX Toolset 3.11+** - Download from [https://wixtoolset.org/releases/](https://wixtoolset.org/releases/)
- **Completed Windows build** - Run `./build-windows.sh` first

## Files

- `Trawsgrifiwr-Byw.wxs` - Main WiX installer configuration
- `license.rtf` - License text shown during installation
- `banner.bmp` (optional) - Custom banner image (493 × 58 pixels)
- `dialog.bmp` (optional) - Custom dialog image (493 × 312 pixels)

## Building the MSI Installer

### On Windows (with WiX installed):

```bash
# 1. Build the Windows application first
./build-windows.sh

# 2. Build the MSI installer
./build-installer.sh
```

The MSI file will be created in `dist/installer/Trawsgrifiwr-Byw-v1.0.0.msi`

### Manual Build (if script doesn't work):

```cmd
cd installer
candle.exe Trawsgrifiwr-Byw.wxs -arch x64
light.exe Trawsgrifiwr-Byw.wixobj -ext WixUIExtension -out Trawsgrifiwr-Byw.msi
```

## Building from macOS/Linux

Since WiX requires Windows, you have these options:

1. **Use a Windows VM** - Run the build script in a Windows virtual machine
2. **Use GitHub Actions** - Set up automated builds (see example below)
3. **Cross-platform alternative** - Consider using Inno Setup or NSIS

## GitHub Actions (Automated Builds)

**Good news!** GitHub Actions workflows are already set up in this repository!

See [../.github/GITHUB_ACTIONS.md](../.github/GITHUB_ACTIONS.md) for complete instructions.

### Quick Start

To trigger an automated build with MSI installer:

```bash
# Create and push a version tag
git tag -a v1.0.0 -m "Release version 1.0.0"
git push origin v1.0.0
```

GitHub Actions will automatically:
- ✅ Build Windows executable + MSI installer
- ✅ Build macOS .app bundle
- ✅ Build Linux executable
- ✅ Create a GitHub Release with all files

The MSI installer will be available in the **Releases** section of your GitHub repository.

For more details, see the [GitHub Actions Setup Guide](../.github/GITHUB_ACTIONS.md).

## Installation Features

The MSI installer includes:

- ✅ Installation to Program Files
- ✅ Start Menu shortcut
- ✅ Desktop shortcut
- ✅ Add/Remove Programs entry
- ✅ Automatic uninstaller
- ✅ Upgrade support (newer versions replace older ones)
- ✅ Per-machine installation

## Customization

### Change Version Number

Edit `Trawsgrifiwr-Byw.wxs` and update:

```xml
<Product Version="1.0.0" ...>
```

Also update `build-installer.sh`:

```bash
VERSION="1.0.0"
```

### Add Custom Images

1. Create `banner.bmp` (493 × 58 pixels)
2. Create `dialog.bmp` (493 × 312 pixels)
3. Place them in the `installer/` directory
4. The WiX configuration will automatically use them

### Change Installation Directory

Edit the `INSTALLFOLDER` in `Trawsgrifiwr-Byw.wxs`:

```xml
<Directory Id="INSTALLFOLDER" Name="Trawsgrifiwr Byw" />
```

## Troubleshooting

### "WiX Toolset not found"

Install WiX from [https://wixtoolset.org/releases/](https://wixtoolset.org/releases/) and ensure it's in your PATH.

### "Windows build not found"

Run `./build-windows.sh` first to create the Windows executable.

### "Cannot build components" error

Make sure all referenced files in the `.wxs` file exist in the `dist/windows-x64` directory.

## Distribution

Once built, the MSI installer can be:

- Distributed to Windows users directly
- Uploaded to GitHub Releases
- Hosted on a website for download
- Signed with a code signing certificate (recommended for production)

## Code Signing (Optional but Recommended)

For production releases, sign the MSI with a certificate:

```cmd
signtool sign /f YourCertificate.pfx /p YourPassword /t http://timestamp.digicert.com Trawsgrifiwr-Byw.msi
```

This prevents Windows SmartScreen warnings.
