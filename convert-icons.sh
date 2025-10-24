#!/bin/bash

# Script to convert SVG icon to platform-specific formats
# Requires ImageMagick: brew install imagemagick

set -e

SVG_FILE="src/Assets/icon.svg"
ASSETS_DIR="src/Assets"

echo "========================================="
echo "Converting Icons for All Platforms"
echo "========================================="
echo ""

# Check if ImageMagick is installed
if ! command -v magick &> /dev/null; then
    echo "ERROR: ImageMagick is not installed!"
    echo ""
    echo "Install it with:"
    echo "  macOS:   brew install imagemagick"
    echo "  Ubuntu:  sudo apt-get install imagemagick"
    echo "  Fedora:  sudo dnf install imagemagick"
    echo ""
    echo "Alternatively, use online converters (see ADDING_ICONS.md)"
    exit 1
fi

# Check if SVG exists
if [ ! -f "$SVG_FILE" ]; then
    echo "ERROR: $SVG_FILE not found!"
    exit 1
fi

cd "$ASSETS_DIR"

echo "Generating PNG files at various sizes..."
magick icon.svg -resize 1024x1024 icon-1024.png
magick icon.svg -resize 512x512 icon-512.png
magick icon.svg -resize 256x256 icon-256.png
magick icon.svg -resize 128x128 icon-128.png
magick icon.svg -resize 64x64 icon-64.png
magick icon.svg -resize 32x32 icon-32.png
magick icon.svg -resize 16x16 icon-16.png

echo "Creating Windows .ico file..."
magick icon-16.png icon-32.png icon-64.png icon-128.png icon-256.png icon.ico

echo "Creating Linux icon.png..."
cp icon-512.png icon.png

# macOS .icns creation (only on macOS)
if [[ "$OSTYPE" == "darwin"* ]]; then
    echo "Creating macOS .icns file..."
    mkdir -p icon.iconset
    cp icon-16.png icon.iconset/icon_16x16.png
    cp icon-32.png icon.iconset/icon_16x16@2x.png
    cp icon-32.png icon.iconset/icon_32x32.png
    cp icon-64.png icon.iconset/icon_32x32@2x.png
    cp icon-128.png icon.iconset/icon_128x128.png
    cp icon-256.png icon.iconset/icon_128x128@2x.png
    cp icon-256.png icon.iconset/icon_256x256.png
    cp icon-512.png icon.iconset/icon_256x256@2x.png
    cp icon-512.png icon.iconset/icon_512x512.png
    cp icon-1024.png icon.iconset/icon_512x512@2x.png
    iconutil -c icns icon.iconset -o icon.icns
    rm -rf icon.iconset
    echo "  ✓ icon.icns created"
else
    echo "Skipping .icns creation (macOS only)"
    echo "  To create .icns for macOS, run this script on a Mac"
fi

# Cleanup intermediate PNG files
echo ""
echo "Cleaning up intermediate files..."
rm -f icon-16.png icon-32.png icon-64.png icon-128.png icon-256.png icon-512.png icon-1024.png

echo ""
echo "========================================="
echo "Icons Created Successfully!"
echo "========================================="
echo ""
echo "Files created in $ASSETS_DIR/:"
ls -lh icon.ico icon.png icon.icns 2>/dev/null || ls -lh icon.ico icon.png 2>/dev/null
echo ""
echo "Next steps:"
echo "1. Add icons to VoskWelshSpeechRecognition.csproj (see ADDING_ICONS.md)"
echo "2. Rebuild: ./build-all.sh"
echo ""
