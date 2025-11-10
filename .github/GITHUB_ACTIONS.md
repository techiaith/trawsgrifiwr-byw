# GitHub Actions Setup Guide

This guide explains how to use GitHub Actions to automatically build Trawsgrifiwr Byw for all platforms, including the Windows MSI installer.

## Overview

We have two GitHub Actions workflows:

1. **Build Test** (`.github/workflows/build-test.yml`) - Runs on every push to test builds
2. **Build Release** (`.github/workflows/build-release.yml`) - Creates releases with all platform builds + MSI installer

## Initial Setup

### 1. Push Workflows to GitHub

The workflow files are already in your repository at:
- `.github/workflows/build-test.yml`
- `.github/workflows/build-release.yml`

Simply commit and push them:

```bash
git add .github/
git commit -m "Add GitHub Actions workflows for automated builds"
git push origin main
```

### 2. Enable GitHub Actions

1. Go to your GitHub repository
2. Click on the **Actions** tab
3. If prompted, enable GitHub Actions for your repository
4. You should see the workflows listed

## Creating a Release

There are two ways to trigger a release build:

### Method 1: Create a Git Tag (Recommended)

```bash
# Create and push a version tag
git tag -a v1.0.0 -m "Release version 1.0.0"
git push origin v1.0.0
```

This will automatically:
1. Build Windows executable + MSI installer
2. Build macOS .app bundle
3. Build Linux executable
4. Create a GitHub Release with all files attached

### Method 2: Manual Trigger from GitHub UI

1. Go to **Actions** tab in your repository
2. Click **Build Release** workflow
3. Click **Run workflow**
4. Enter the version number (e.g., `1.0.0`)
5. Click **Run workflow**

This builds all platforms but does NOT create a GitHub Release (artifacts only).

## What Gets Built

### Windows Build
- **Executable**: `Trawsgrifiwr-Byw.exe` (single file, ~50-60MB)
- **MSI Installer**: `Trawsgrifiwr-Byw-v1.0.0.msi` (professional installer)
- **ZIP Archive**: `Trawsgrifiwr-Byw-Windows-x64-v1.0.0.zip` (portable version)

### macOS Build
- **App Bundle**: `Trawsgrifiwr Byw.app` (double-clickable)
- **ZIP Archive**: `Trawsgrifiwr-Byw-macOS-osx-arm64-v1.0.0.zip` (for Apple Silicon)

### Linux Build
- **Executable**: `Trawsgrifiwr-Byw` (single file)
- **TAR.GZ Archive**: `Trawsgrifiwr-Byw-Linux-x64-v1.0.0.tar.gz`

## Viewing Build Results

### For Test Builds (on every push)

1. Go to **Actions** tab
2. Click on the latest workflow run
3. View the build status for each platform
4. Check logs for any errors

### For Release Builds (on tags)

1. Go to **Actions** tab to see build progress
2. Once complete, go to **Releases** tab
3. You'll see your new release with all download files attached
4. Download the MSI installer from the release page

## Downloading Build Artifacts

If you manually triggered the workflow or want to test builds before creating a release:

1. Go to **Actions** tab
2. Click on the workflow run
3. Scroll down to **Artifacts** section
4. Download:
   - `windows-x64` - Contains the ZIP archive
   - `windows-msi-installer` - Contains the MSI file
   - `macos-build` - Contains the macOS ZIP archive
   - `linux-x64` - Contains the Linux TAR.GZ

Artifacts are kept for 90 days by default.

## Release Workflow Details

The Build Release workflow (`build-release.yml`) performs these steps:

### Windows Job
1. Checks out code
2. Installs .NET 8.0
3. Installs WiX Toolset v5.0
4. Builds Windows executable using `build-windows.sh`
5. Builds MSI installer using WiX
6. Creates ZIP archive
7. Uploads artifacts

### macOS Job
1. Checks out code
2. Installs .NET 8.0
3. Builds macOS .app bundle using `build-macos.sh`
4. Creates ZIP archive
5. Uploads artifacts

### Linux Job
1. Checks out code
2. Installs .NET 8.0
3. Builds Linux executable using `build-linux.sh`
4. Creates TAR.GZ archive
5. Uploads artifacts

### Create Release Job
1. Downloads all build artifacts
2. Creates GitHub Release (only if triggered by tag)
3. Uploads all files to the release
4. Generates release notes automatically

## Customizing Workflows

### Change Version Number

The version is automatically extracted from the git tag. For example:
- Tag `v1.0.0` → Version `1.0.0`
- Tag `v2.1.3` → Version `2.1.3`

To use a different versioning scheme, edit the workflow files.

### Change Trigger Events

Edit `.github/workflows/build-release.yml`:

```yaml
on:
  push:
    tags:
      - 'v*'           # Any tag starting with 'v'
      - 'release/*'    # Or tags like 'release/1.0.0'
```

### Add Code Signing (Optional)

To sign the Windows MSI (prevents SmartScreen warnings):

1. Get a code signing certificate
2. Add it to GitHub Secrets:
   - Go to **Settings** → **Secrets and variables** → **Actions**
   - Add secrets: `SIGNING_CERT` (base64 of .pfx), `SIGNING_PASSWORD`
3. Update workflow to sign the MSI:

```yaml
- name: Sign MSI
  run: |
    echo "${{ secrets.SIGNING_CERT }}" | base64 --decode > cert.pfx
    signtool sign /f cert.pfx /p "${{ secrets.SIGNING_PASSWORD }}" /t http://timestamp.digicert.com dist/installer/*.msi
    rm cert.pfx
```

## Troubleshooting

### Build Fails on Windows

Check the logs in GitHub Actions. Common issues:
- Missing dependencies in WiX configuration
- File paths don't match actual build output
- WiX version compatibility

### Build Fails on macOS

- Ensure the build script correctly detects architecture (arm64 vs x64)
- Check that the .app bundle is created correctly

### MSI Not Created

- Check that Windows build completed successfully first
- Verify WiX installation logs
- Ensure all files referenced in `Trawsgrifiwr-Byw.wxs` exist

### Release Not Created

- Ensure you pushed a tag (not just committed)
- Check that all build jobs completed successfully
- Verify the `create-release` job ran

## Local Testing

Test the workflows locally before pushing:

```bash
# Install act (GitHub Actions local runner)
brew install act  # macOS
# or
sudo apt install act  # Linux

# Run the test workflow
act -j test-build-windows
act -j test-build-macos
act -j test-build-linux
```

Note: `act` may not perfectly replicate GitHub's environment, but it's useful for catching basic errors.

## Best Practices

### 1. Use Semantic Versioning

Follow [SemVer](https://semver.org/):
- `v1.0.0` - Major release
- `v1.1.0` - Minor (new features)
- `v1.0.1` - Patch (bug fixes)

### 2. Test Before Tagging

```bash
# Push to main first
git push origin main

# Wait for test builds to pass in Actions tab

# Then create release tag
git tag -a v1.0.0 -m "Release 1.0.0"
git push origin v1.0.0
```

### 3. Write Good Release Notes

When creating a tag with a message:

```bash
git tag -a v1.0.0 -m "Release 1.0.0

New Features:
- Feature 1
- Feature 2

Bug Fixes:
- Fix 1
- Fix 2

Breaking Changes:
- Change 1
"
```

This message will be included in the GitHub Release.

### 4. Keep Artifacts Clean

Delete old workflow runs to save storage:
1. Go to **Actions** tab
2. Click **...** on old runs
3. Select **Delete workflow run**

## GitHub Actions Costs

- **Public repositories**: GitHub Actions is FREE with unlimited minutes
- **Private repositories**: 2,000 free minutes/month, then paid

Your builds take approximately:
- Windows: ~5-10 minutes
- macOS: ~5-10 minutes
- Linux: ~3-5 minutes

Total: ~15-25 minutes per release

## Next Steps

1. **Push the workflows to GitHub**
   ```bash
   git add .github/
   git commit -m "Add automated build workflows"
   git push
   ```

2. **Create your first release**
   ```bash
   git tag -a v1.0.0 -m "Initial release"
   git push origin v1.0.0
   ```

3. **Check the Actions tab** to see your builds in progress

4. **Download from Releases tab** once complete

## Support

If you encounter issues:

1. Check the **Actions** tab for build logs
2. Look for error messages in red
3. Review this guide for common solutions
4. Check GitHub's [Actions documentation](https://docs.github.com/en/actions)

Happy releasing! 🚀
