#!/bin/bash

# Create distribution archives for releases
# Packages each platform build into a zip/tar.gz file

set -e  # Exit on error

echo "========================================="
echo "Creating Release Archives"
echo "========================================="
echo ""

RELEASE_DIR="releases"
VERSION="1.0.0"  # Update this for each release

# Create releases directory
mkdir -p "$RELEASE_DIR"

# Detect macOS architecture
ARCH=$(uname -m)
if [ "$ARCH" = "arm64" ]; then
    MACOS_RID="osx-arm64"
else
    MACOS_RID="osx-x64"
fi

echo "Creating archives..."
echo ""

# macOS
if [ -d "dist/macos-$MACOS_RID" ]; then
    echo "Packaging macOS ($MACOS_RID)..."
    cd dist/macos-$MACOS_RID
    zip -r "../../$RELEASE_DIR/Trawsgrifiwr-Byw-macOS-$MACOS_RID-v$VERSION.zip" .
    cd ../..
    echo "  ✓ Created: $RELEASE_DIR/Trawsgrifiwr-Byw-macOS-$MACOS_RID-v$VERSION.zip"
fi

# Windows
if [ -d "dist/windows-x64" ]; then
    echo "Packaging Windows..."
    cd dist/windows-x64
    zip -r "../../$RELEASE_DIR/Trawsgrifiwr-Byw-Windows-x64-v$VERSION.zip" .
    cd ../..
    echo "  ✓ Created: $RELEASE_DIR/Trawsgrifiwr-Byw-Windows-x64-v$VERSION.zip"
fi

# Linux
if [ -d "dist/linux-x64" ]; then
    echo "Packaging Linux..."
    cd dist/linux-x64
    tar -czf "../../$RELEASE_DIR/Trawsgrifiwr-Byw-Linux-x64-v$VERSION.tar.gz" .
    cd ../..
    echo "  ✓ Created: $RELEASE_DIR/Trawsgrifiwr-Byw-Linux-x64-v$VERSION.tar.gz"
fi

# Windows MSI Installer
if [ -f "dist/installer/Trawsgrifiwr-Byw-v$VERSION.msi" ]; then
    echo "Copying Windows MSI Installer..."
    cp "dist/installer/Trawsgrifiwr-Byw-v$VERSION.msi" "$RELEASE_DIR/"
    echo "  ✓ Created: $RELEASE_DIR/Trawsgrifiwr-Byw-v$VERSION.msi"
else
    echo "Note: Windows MSI installer not found. Run ./build-installer.sh on Windows to create it."
fi

echo ""
echo "========================================="
echo "Release Archives Created!"
echo "========================================="
echo ""
echo "Archives in: $RELEASE_DIR/"
ls -lh "$RELEASE_DIR/"
echo ""
echo "Ready for distribution!"
echo ""
