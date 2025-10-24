#!/bin/bash

# Build script for Linux
# Creates a self-contained Linux executable

set -e  # Exit on error

echo "========================================="
echo "Building Vosk Welsh Speech Recognition"
echo "Platform: Linux x64"
echo "========================================="

# Project configuration
PROJECT_DIR="src"
OUTPUT_DIR="dist/linux-x64"
APP_NAME="VoskWelshSpeechRecognition"

echo ""
echo "Cleaning previous builds..."
rm -rf "$OUTPUT_DIR"
dotnet clean "$PROJECT_DIR" --configuration Release

echo ""
echo "Publishing application..."
dotnet publish "$PROJECT_DIR" \
    --configuration Release \
    --runtime linux-x64 \
    --self-contained true \
    --output "$OUTPUT_DIR" \
    -p:PublishSingleFile=true \
    -p:IncludeNativeLibrariesForSelfExtract=true \
    -p:EnableCompressionInSingleFile=true \
    -p:DebugType=None \
    -p:DebugSymbols=false

echo ""
echo "Making executable..."
chmod +x "$OUTPUT_DIR/$APP_NAME"

echo ""
echo "Build completed successfully!"
echo ""
echo "Output location: $OUTPUT_DIR"
echo "Executable: $OUTPUT_DIR/$APP_NAME"
echo ""
echo "File size:"
ls -lh "$OUTPUT_DIR/$APP_NAME" | awk '{print $5, $9}'
echo ""
echo "To run on Linux:"
echo "  cd $OUTPUT_DIR"
echo "  ./$APP_NAME"
echo ""
echo "Note: Linux users may need to install OpenAL:"
echo "  Ubuntu/Debian: sudo apt-get install libopenal1"
echo "  Fedora/RHEL: sudo dnf install openal-soft"
echo "  Arch: sudo pacman -S openal"
echo ""
echo "========================================="
