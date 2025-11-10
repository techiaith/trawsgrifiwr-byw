#!/bin/bash

# Build Windows MSI Installer using WiX Toolset
# This script must be run on Windows with WiX Toolset installed
# or on Linux/macOS with WiX installed via mono/wine

set -e  # Exit on error

echo "========================================="
echo "Building Trawsgrifiwr Byw MSI Installer"
echo "========================================="
echo ""

# Check if running on Windows or need to use alternative method
if [[ "$OSTYPE" == "msys" || "$OSTYPE" == "win32" || "$OSTYPE" == "cygwin" ]]; then
    # Running on Windows
    WIX_CANDLE="candle.exe"
    WIX_LIGHT="light.exe"
else
    echo "Note: This script is designed to run on Windows with WiX Toolset."
    echo "For macOS/Linux, you need to:"
    echo "  1. Build the Windows executable first: ./build-windows.sh"
    echo "  2. Copy the dist/windows-x64 folder to a Windows machine"
    echo "  3. Install WiX Toolset: https://wixtoolset.org/releases/"
    echo "  4. Run this script on Windows"
    echo ""
    echo "Alternatively, use GitHub Actions or a Windows VM."
    exit 1
fi

# Configuration
INSTALLER_DIR="installer"
OUTPUT_DIR="dist/installer"
VERSION="1.0.0"

# Check if WiX is installed
if ! command -v $WIX_CANDLE &> /dev/null; then
    echo "Error: WiX Toolset not found!"
    echo "Please install WiX Toolset from: https://wixtoolset.org/releases/"
    echo "Make sure the WiX bin directory is in your PATH"
    exit 1
fi

# Check if Windows build exists
if [ ! -f "dist/windows-x64/Trawsgrifiwr-Byw.exe" ]; then
    echo "Error: Windows build not found!"
    echo "Please run ./build-windows.sh first"
    exit 1
fi

# Create output directory
mkdir -p "$OUTPUT_DIR"

echo "Compiling WiX source..."
$WIX_CANDLE "$INSTALLER_DIR/Trawsgrifiwr-Byw.wxs" \
    -out "$OUTPUT_DIR/Trawsgrifiwr-Byw.wixobj" \
    -arch x64

echo ""
echo "Linking MSI installer..."
$WIX_LIGHT "$OUTPUT_DIR/Trawsgrifiwr-Byw.wixobj" \
    -out "$OUTPUT_DIR/Trawsgrifiwr-Byw-v$VERSION.msi" \
    -ext WixUIExtension \
    -cultures:en-us

echo ""
echo "========================================="
echo "MSI Installer Created Successfully!"
echo "========================================="
echo ""
echo "Output: $OUTPUT_DIR/Trawsgrifiwr-Byw-v$VERSION.msi"
echo ""
echo "File size:"
ls -lh "$OUTPUT_DIR/Trawsgrifiwr-Byw-v$VERSION.msi" | awk '{print $5, $9}'
echo ""
echo "To install:"
echo "  1. Copy the MSI file to a Windows machine"
echo "  2. Double-click to run the installer"
echo "  3. Follow the installation wizard"
echo ""
echo "========================================="
