#!/bin/bash

# Build script for all platforms
# Creates self-contained executables for macOS, Windows, and Linux

set -e  # Exit on error

echo "========================================="
echo "Building for ALL Platforms"
echo "========================================="
echo ""

# Build macOS
echo ">>> Building macOS version..."
./build-macos.sh
echo ""

# Build Windows
echo ">>> Building Windows version..."
./build-windows.sh
echo ""

# Build Linux
echo ">>> Building Linux version..."
./build-linux.sh
echo ""

echo "========================================="
echo "ALL BUILDS COMPLETED!"
echo "========================================="
echo ""
echo "Build outputs:"
echo "  macOS:   dist/macos-*/"
echo "  Windows: dist/windows-x64/"
echo "  Linux:   dist/linux-x64/"
echo ""
echo "Total sizes:"
du -sh dist/*/
echo ""
echo "To create distribution archives:"
echo "  ./create-releases.sh"
echo ""
