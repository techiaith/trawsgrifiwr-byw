# Quick Icon Setup (5 Minutes)

## Easiest Method - Use Online Converter

### Step 1: Get Icon Files

Visit these links and upload `src/Assets/icon.svg`:

1. **macOS icon**: https://cloudconvert.com/svg-to-icns
   - Upload `src/Assets/icon.svg`
   - Click "Convert"
   - Download and save as `src/Assets/icon.icns`

2. **Windows icon**: https://cloudconvert.com/svg-to-ico
   - Upload `src/Assets/icon.svg`
   - Click "Convert"
   - Download and save as `src/Assets/icon.ico`

3. **Linux icon**: https://cloudconvert.com/svg-to-png
   - Upload `src/Assets/icon.svg`
   - Set width/height to **512**
   - Click "Convert"
   - Download and save as `src/Assets/icon.png`

### Step 2: Update Project File

Edit `src/VoskWelshSpeechRecognition.csproj` and add this inside the `<PropertyGroup>`:

```xml
<!-- Application Icon -->
<ApplicationIcon>Assets/icon.ico</ApplicationIcon>
```

And add this as a new `<ItemGroup>` section (after the existing ItemGroups):

```xml
<!-- Include icon files -->
<ItemGroup>
  <None Include="Assets/icon.ico" />
  <None Include="Assets/icon.icns">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </None>
  <None Include="Assets/icon.png">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

### Step 3: Rebuild

```bash
./build-macos.sh
```

Done! Your app will now have a custom icon.

---

## Alternative: Use a Custom Design

Don't like the default icon? Design your own:

1. Use **Canva** (free): https://www.canva.com/
   - Create a 1024x1024 design
   - Export as PNG or SVG

2. Or search for free icons:
   - https://www.flaticon.com/ (search "microphone" or "speech")
   - https://icons8.com/icons/set/microphone

3. Save to `src/Assets/icon.svg` or `icon.png`

4. Convert using the online tools above

---

## Want a Welsh Dragon Icon?

Generate one with AI:

1. Go to https://www.bing.com/create (free)
2. Prompt: "Simple minimalist Welsh red dragon icon on green background, clean vector style, app icon"
3. Download the image
4. Convert using cloudconvert.com
5. Save to `src/Assets/`

Much more professional! 🏴󠁧󠁢󠁷󠁬󠁳󠁿
