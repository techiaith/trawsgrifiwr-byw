# Troubleshooting Guide

## Model Download Failures

If the Welsh language model fails to download when you first run the application, detailed error information is available.

### Where to Find Error Logs

When a download error occurs:

1. **Error Dialog**: A detailed error dialog will appear showing:
   - The specific error message
   - Common causes of the problem
   - Location of the log file

2. **Log File Location**:
   - **Windows**: `%LOCALAPPDATA%\Trawsgrifiwr-Byw\download-error.log`
     - Full path example: `C:\Users\YourName\AppData\Local\Trawsgrifiwr-Byw\download-error.log`
   - **macOS**: `~/Library/Application Support/Trawsgrifiwr-Byw/download-error.log`
   - **Linux**: `~/.local/share/Trawsgrifiwr-Byw/download-error.log`

   The exact path is shown in the error dialog.

3. **Opening the Log**: Click the "Open Log File" button in the error dialog to view the detailed logs.

### Common Download Errors

#### 1. No Internet Connection
**Symptoms:**
- Error: `No such host is known` or `Unable to connect`

**Solutions:**
- Check your internet connection
- Try accessing https://huggingface.co/ in a web browser
- Disable VPN temporarily if using one

#### 2. Firewall Blocking
**Symptoms:**
- Error: `The operation has timed out` or `Connection refused`

**Solutions:**
- Temporarily disable firewall
- Add Trawsgrifiwr Byw to firewall exceptions
- Check corporate/network firewall settings

#### 3. Antivirus Blocking
**Symptoms:**
- Error: `Access denied` or `UnauthorizedAccessException`
- Download completes but extraction fails

**Solutions:**
- Temporarily disable antivirus
- Add Trawsgrifiwr Byw to antivirus exclusions
- Add the Models folder to exclusions

#### 4. Insufficient Disk Space
**Symptoms:**
- Error: `There is not enough space on the disk`
- Download starts but fails partway through

**Solutions:**
- Free up at least 100MB of disk space
- The model is approximately 47MB compressed, 70MB extracted
- Check the drive where the app is installed

#### 5. Corrupted Download
**Symptoms:**
- Download completes but extraction fails
- Error: `The archive is corrupted` or `Invalid data`

**Solutions:**
- Delete the partially downloaded file manually:
  - **Windows**: `%LOCALAPPDATA%\Trawsgrifiwr-Byw\Models\model_cy.tar.gz`
    - Type `%LOCALAPPDATA%` in File Explorer address bar to navigate there
  - **macOS**: `~/Library/Application Support/Trawsgrifiwr-Byw/Models/model_cy.tar.gz`
  - **Linux**: `~/.local/share/Trawsgrifiwr-Byw/Models/model_cy.tar.gz`
- Restart the application to retry the download

#### 6. Hugging Face Server Issues
**Symptoms:**
- Error: `503 Service Unavailable` or `502 Bad Gateway`
- HTTP status codes 500-599

**Solutions:**
- Wait 15-30 minutes and try again
- Check https://status.huggingface.co/ for service status
- Try downloading manually (see below)

### Manual Model Download

If automatic download continues to fail, you can download the model manually:

1. **Download the model**:
   - Visit: https://huggingface.co/techiaith/kaldi-cy/resolve/main/model_cy.tar.gz
   - Save the file to your computer

2. **Create the Models folder**:
   - **Windows**: `%LOCALAPPDATA%\Trawsgrifiwr-Byw\Models`
     - Open File Explorer, type `%LOCALAPPDATA%` in the address bar
     - Create folders: `Trawsgrifiwr-Byw\Models`
   - **macOS**: `~/Library/Application Support/Trawsgrifiwr-Byw/Models`
   - **Linux**: `~/.local/share/Trawsgrifiwr-Byw/Models`

3. **Extract the model**:

   **Windows (using File Explorer):**
   - Use 7-Zip or WinRAR to extract `model_cy.tar.gz`
   - Rename the extracted `model` folder to `vosk-model-cy`
   - Move to `%LOCALAPPDATA%\Trawsgrifiwr-Byw\Models\vosk-model-cy`

   **macOS/Linux:**
   ```bash
   # Extract the tar.gz file
   tar -xzf model_cy.tar.gz

   # Rename the extracted folder
   mv model vosk-model-cy

   # Move to Models directory (macOS)
   mv vosk-model-cy ~/Library/Application\ Support/Trawsgrifiwr-Byw/Models/

   # OR for Linux
   mv vosk-model-cy ~/.local/share/Trawsgrifiwr-Byw/Models/
   ```

4. **Restart the application**

The app should now find the model and start normally.

### Log File Contents

The log file contains:
- Timestamp of each operation
- HTTP request/response details
- File sizes and progress
- Full exception messages and stack traces
- System information

Example log entry:
```
[2025-11-10 14:32:15.123] Starting model download...
[2025-11-10 14:32:15.124] Download URL: https://huggingface.co/techiaith/kaldi-cy/resolve/main/model_cy.tar.gz
[2025-11-10 14:32:15.125] Target directory: C:\Users\...\Models
[2025-11-10 14:32:15.126] Creating Models directory: C:\Users\...\Models
[2025-11-10 14:32:15.127] Models directory created successfully
[2025-11-10 14:32:15.128] Sending GET request to: https://...
[2025-11-10 14:32:16.456] HTTP Response Status: 200
[2025-11-10 14:32:16.457] Content length: 49283847 bytes (47.01MB)
...
```

### Getting Help

If you're still experiencing issues:

1. **Check the log file** for specific error messages
2. **Copy the relevant error lines** from the log
3. **Open an issue** on GitHub with:
   - The error message from the dialog
   - Relevant lines from the log file
   - Your operating system and version
   - Whether you're behind a corporate firewall/proxy

## Other Common Issues

### Recording Not Starting

**Symptoms:**
- Click "Start Recording" but nothing happens
- Error: "Error loading model"

**Solutions:**
- Ensure the model was downloaded successfully
- Check that you've granted microphone permissions
- Restart the application

### No Sound Being Captured

**Symptoms:**
- Recording starts but no transcription appears
- Microphone light is on but nothing happens

**Solutions:**
- Check microphone is selected as default input device
- Test microphone in other applications
- Grant microphone permissions if prompted
- Try a different microphone

### Windows SmartScreen Warning

**Symptoms:**
- "Windows protected your PC" message when installing

**Solutions:**
- This is normal for unsigned applications
- Click "More info" → "Run anyway"
- The application is safe (source code is available on GitHub)
- For signed versions without warnings, see [CODE_SIGNING.md](.github/CODE_SIGNING.md)

## Technical Information

### Application Directories

**Windows:**
- App location: `C:\Program Files\Trawsgrifiwr Byw\`
- Models: `%LOCALAPPDATA%\Trawsgrifiwr-Byw\Models\` (user-writable, no admin required)
  - Example: `C:\Users\YourName\AppData\Local\Trawsgrifiwr-Byw\Models\`
- Logs: `%LOCALAPPDATA%\Trawsgrifiwr-Byw\download-error.log`

**macOS:**
- App location: `/Applications/Trawsgrifiwr Byw.app`
- Models: `~/Library/Application Support/Trawsgrifiwr-Byw/Models/`
- Logs: `~/Library/Application Support/Trawsgrifiwr-Byw/download-error.log`

**Linux:**
- App location: Where you extracted/installed it
- Models: `~/.local/share/Trawsgrifiwr-Byw/Models/`
- Logs: `~/.local/share/Trawsgrifiwr-Byw/download-error.log`

### Model Information

- **Name**: vosk-model-cy (Welsh Kaldi model)
- **Source**: https://huggingface.co/techiaith/kaldi-cy
- **Size**: ~47MB compressed, ~70MB extracted
- **Format**: Kaldi model optimized for Vosk
- **Language**: Welsh (Cymraeg)

### System Requirements

- **OS**: Windows 10+, macOS 10.15+, Linux (modern distros)
- **RAM**: 4GB minimum, 8GB recommended
- **.NET**: 8.0 Runtime (included in self-contained builds)
- **Disk Space**: 100MB free (for model download and extraction)
- **Internet**: Required for first-time model download only
- **Microphone**: Any working microphone/audio input device

---

## Still Need Help?

- **Documentation**: [README.md](README.md)
- **GitHub Issues**: https://github.com/techiaith/trawsgrifiwr-byw/issues
- **Techiaith**: https://techiaith.cymru/

**When reporting issues, please include:**
1. Your operating system and version
2. The error message from the dialog
3. Relevant lines from `download-error.log`
4. Steps to reproduce the problem
