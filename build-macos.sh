#!/bin/bash

# Build script for macOS
# Creates a self-contained application bundle

set -e  # Exit on error

echo "========================================="
echo "Building Vosk Welsh Speech Recognition"
echo "Platform: macOS"
echo "========================================="

# Detect architecture
ARCH=$(uname -m)
if [ "$ARCH" = "arm64" ]; then
    RID="osx-arm64"
    echo "Detected: Apple Silicon (M1/M2/M3)"
elif [ "$ARCH" = "x86_64" ]; then
    RID="osx-x64"
    echo "Detected: Intel Mac"
else
    echo "Unknown architecture: $ARCH"
    exit 1
fi

# Project configuration
PROJECT_DIR="src"
OUTPUT_DIR="dist/macos-$RID"
APP_NAME="Ap Hel Lleferydd"

echo ""
echo "Cleaning previous builds..."
rm -rf "$OUTPUT_DIR"
dotnet clean "$PROJECT_DIR" --configuration Release

echo ""
echo "Publishing application..."
dotnet publish "$PROJECT_DIR" \
    --configuration Release \
    --runtime "$RID" \
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

# Create macOS .app bundle
echo "Creating macOS application bundle..."
APP_BUNDLE="$OUTPUT_DIR/$APP_NAME.app"
CONTENTS_DIR="$APP_BUNDLE/Contents"
MACOS_DIR="$CONTENTS_DIR/MacOS"
RESOURCES_DIR="$CONTENTS_DIR/Resources"

mkdir -p "$MACOS_DIR"
mkdir -p "$RESOURCES_DIR"

# Move executable and libraries into the bundle
# The executable is built with AssemblyName (no spaces)
mv "$OUTPUT_DIR/ApHelLleferydd" "$MACOS_DIR/"
mv "$OUTPUT_DIR"/*.dylib "$MACOS_DIR/" 2>/dev/null || true
mv "$OUTPUT_DIR/Assets" "$MACOS_DIR/" 2>/dev/null || true
mv "$OUTPUT_DIR/Models" "$MACOS_DIR/" 2>/dev/null || true

# Copy icon to Resources if it exists
if [ -f "src/Assets/icon.png" ]; then
    cp "src/Assets/icon.png" "$RESOURCES_DIR/icon.png"
fi

# Create Info.plist
cat > "$CONTENTS_DIR/Info.plist" << 'EOF'
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>CFBundleExecutable</key>
    <string>ApHelLleferydd</string>
    <key>CFBundleIconFile</key>
    <string>icon.png</string>
    <key>CFBundleIdentifier</key>
    <string>com.techiaith.aphellleferydd</string>
    <key>CFBundleName</key>
    <string>Ap Hel Lleferydd</string>
    <key>CFBundleDisplayName</key>
    <string>Ap Hel Lleferydd</string>
    <key>CFBundlePackageType</key>
    <string>APPL</string>
    <key>CFBundleShortVersionString</key>
    <string>1.0.0</string>
    <key>CFBundleVersion</key>
    <string>1.0.0</string>
    <key>LSMinimumSystemVersion</key>
    <string>10.15</string>
    <key>NSHighResolutionCapable</key>
    <true/>
    <key>NSMicrophoneUsageDescription</key>
    <string>Mae'r ap hwn angen mynediad i'r meicroffon ar gyfer adnabod lleferydd Cymraeg.</string>
</dict>
</plist>
EOF

echo "  ✓ Created application bundle: $APP_NAME.app"
echo ""
echo "Output location: $OUTPUT_DIR"
echo "Application bundle: $APP_BUNDLE"
echo ""
echo "To run the application:"
echo "  open $OUTPUT_DIR/$APP_NAME.app"
echo ""
echo "Or double-click the .app in Finder!"
echo ""
echo "Bundle size:"
du -sh "$APP_BUNDLE" | awk '{print $1, $2}'
echo ""
echo "========================================="
