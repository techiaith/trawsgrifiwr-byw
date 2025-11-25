#!/bin/bash

# Build macOS installer (.pkg) for Trawsgrifiwr Byw
# This script creates a .pkg installer from the built .app bundle

set -e  # Exit on error

# Detect architecture
ARCH=$(uname -m)
if [ "$ARCH" = "arm64" ]; then
    RID="osx-arm64"
    ARCH_NAME="Apple-Silicon"
elif [ "$ARCH" = "x86_64" ]; then
    RID="osx-x64"
    ARCH_NAME="Intel"
else
    echo "Unknown architecture: $ARCH"
    exit 1
fi

# Configuration
APP_NAME="Trawsgrifiwr Byw"
APP_BUNDLE="../../dist/macos-$RID/$APP_NAME.app"
IDENTIFIER="com.techiaith.trawsgrifiwr-byw"
VERSION="${1:-1.0.0}"  # Version passed as argument, defaults to 1.0.0
INSTALLER_DIR="$(pwd)"
PKG_ROOT="$INSTALLER_DIR/pkg-root"
PKG_OUTPUT="../../dist/installer"
PKG_NAME="Trawsgrifiwr-Byw-macOS-$ARCH_NAME-v$VERSION.pkg"

echo "========================================="
echo "Building macOS Installer Package"
echo "========================================="
echo "Architecture: $ARCH_NAME ($RID)"
echo "Version: $VERSION"
echo "Package: $PKG_NAME"
echo ""

# Check if app bundle exists
if [ ! -d "$APP_BUNDLE" ]; then
    echo "Error: App bundle not found at: $APP_BUNDLE"
    echo "Please run build-macos.sh first to build the application."
    exit 1
fi

# Clean and create directories
echo "Preparing directories..."
rm -rf "$PKG_ROOT"
mkdir -p "$PKG_ROOT/Applications"
mkdir -p "$PKG_OUTPUT"

# Copy app bundle to package root
echo "Copying application bundle..."
cp -R "$APP_BUNDLE" "$PKG_ROOT/Applications/"

# Ad-hoc sign the app bundle (helps reduce Gatekeeper warnings)
echo "Signing application bundle..."
if codesign --force --deep --sign - "$PKG_ROOT/Applications/$APP_NAME.app" 2>/dev/null; then
    echo "  ✓ Application signed with ad-hoc signature"
else
    echo "  ⚠ Could not sign application"
fi

# Make scripts executable
echo "Setting up installation scripts..."
chmod +x scripts/postinstall

# Build component package
echo "Building component package..."
pkgbuild --root "$PKG_ROOT" \
         --identifier "$IDENTIFIER" \
         --version "$VERSION" \
         --scripts scripts \
         --install-location "/" \
         trawsgrifiwr-byw.pkg

# Build product package with custom installer UI
echo "Building product installer..."
productbuild --distribution Distribution.xml \
             --resources . \
             --package-path . \
             "$PKG_OUTPUT/$PKG_NAME"

# Clean up intermediate files
echo "Cleaning up..."
rm -f trawsgrifiwr-byw.pkg
rm -rf "$PKG_ROOT"

echo ""
echo "========================================="
echo "Installer created successfully!"
echo "========================================="
echo "Location: $PKG_OUTPUT/$PKG_NAME"
echo ""
echo "File size:"
ls -lh "$PKG_OUTPUT/$PKG_NAME" | awk '{print $5}'
echo ""
echo "To install:"
echo "  open $PKG_OUTPUT/$PKG_NAME"
echo ""
echo "Or double-click the .pkg file in Finder"
echo "========================================="
