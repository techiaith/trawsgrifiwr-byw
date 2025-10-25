#!/bin/bash

# Build script for Windows
# Creates a self-contained Windows executable

set -e  # Exit on error

echo "========================================="
echo "Building Ap Hel Lleferydd"
echo "Platform: Windows x64"
echo "========================================="

# Project configuration
PROJECT_DIR="src"
OUTPUT_DIR="dist/windows-x64"
APP_NAME="ApHelLleferydd.exe"

echo ""
echo "Cleaning previous builds..."
rm -rf "$OUTPUT_DIR"
dotnet clean "$PROJECT_DIR" --configuration Release

echo ""
echo "Publishing application..."
dotnet publish "$PROJECT_DIR" \
    --configuration Release \
    --runtime win-x64 \
    --self-contained true \
    --output "$OUTPUT_DIR" \
    -p:PublishSingleFile=true \
    -p:IncludeNativeLibrariesForSelfExtract=true \
    -p:EnableCompressionInSingleFile=true \
    -p:DebugType=None \
    -p:DebugSymbols=false

echo ""
echo "Build completed successfully!"
echo ""
echo "Output location: $OUTPUT_DIR"
echo "Executable: $OUTPUT_DIR/$APP_NAME"
echo ""
echo "File size:"
ls -lh "$OUTPUT_DIR/$APP_NAME" | awk '{print $5, $9}'
echo ""
echo "To test on Windows:"
echo "  1. Copy the $OUTPUT_DIR folder to a Windows machine"
echo "  2. Double-click $APP_NAME"
echo ""
echo "========================================="
