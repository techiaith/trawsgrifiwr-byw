# Accessibility Features

## Overview

Ap Hel Lleferydd / Welsh Speech Recognition Application has been designed with comprehensive accessibility features to ensure it can be used by everyone, including blind and visually impaired users who rely on screen readers.

## Screen Reader Support

### Labeled Controls

All interactive elements in the application have proper accessibility labels:

- **Buttons**: Each button announces its purpose clearly
  - Start Recording button
  - Stop Recording button
  - Clear Transcriptions button

- **Text Areas**: All text display areas are properly labeled
  - Live partial transcription area
  - Completed transcriptions area
  - Application status indicator

### Live Region Announcements

The application uses ARIA live regions to announce important updates to screen readers in real-time:

#### Assertive Announcements (Immediate)
These interrupt the screen reader to provide critical information:
- "Application ready. Press F5 to start recording."
- "Recording started. Speak in Welsh. Press F6 to stop."
- "Recording stopped. Ready to record again. Press F5 to start."
- "All transcriptions cleared."
- Error messages

#### Polite Announcements (Non-Interrupting)
These are announced when the screen reader is idle:
- Each completed transcription: "Transcribed: [text]"
- Status updates during initialization

### Status Updates

All visual status changes are announced to screen readers:
- Application initialization status
- Model downloading progress
- Recording state changes
- Error conditions

## Keyboard Navigation

### Keyboard Shortcuts

The application provides function key shortcuts for all main actions:

| Key | Action | Description |
|-----|--------|-------------|
| **F5** | Start Recording | Begins speech recognition |
| **F6** | Stop Recording | Stops speech recognition |
| **F7** | Clear | Clears all transcription text |

### Tab Navigation

All interactive controls can be reached using the Tab key:
1. Start Recording button
2. Stop Recording button
3. Clear button

### Visual Focus Indicators

When navigating with the keyboard, focused elements display:
- **Gold border** (3px thickness) around the focused button
- High contrast to ensure visibility

## Focus Management

The application intelligently manages keyboard focus to improve usability:

- **On Startup**: Focus automatically moves to the Start Recording button when the application is ready
- **When Recording Starts**: Focus moves to the Stop Recording button
- **When Recording Stops**: Focus returns to the Start Recording button
- This ensures users can quickly perform the next logical action

## Real-Time Transcription Accessibility

### Live Input Display
- Shows partial transcription as you speak
- Updates announced with "polite" priority to avoid interrupting
- Screen readers can read current content on demand

### Completed Transcriptions
- Each completed sentence is announced: "Transcribed: [sentence]"
- Timestamped entries for context
- Auto-scrolls to show latest transcription
- Screen reader can read full history

## Help Text

All interactive controls include contextual help text that screen readers can access:
- Button descriptions explain what each button does
- Keyboard shortcuts are documented in the help text
- Section descriptions explain the purpose of each area

## Bilingual Support

All labels and announcements are provided in both:
- **Welsh (Cymraeg)**: Primary language
- **English**: For accessibility

This ensures both Welsh and English-speaking users can use the application effectively.

## AutomationProperties Implementation

The application uses Avalonia's AutomationProperties extensively:

```xml
AutomationProperties.Name - Clear, descriptive labels
AutomationProperties.HelpText - Detailed usage information
AutomationProperties.LiveSetting - For real-time announcements
AutomationProperties.AccessKey - Documents keyboard shortcuts
```

## Testing with Screen Readers

This application has been designed to work with popular screen readers:
- **Windows**: NVDA, JAWS, Windows Narrator
- **macOS**: VoiceOver
- **Linux**: Orca

## Accessibility Standards

The application follows these accessibility guidelines:
- **WCAG 2.1 Level AA** compliance
- Proper ARIA live regions
- Keyboard accessibility
- Focus management
- Screen reader compatibility

## Usage Tips for Screen Reader Users

1. **Starting the Application**:
   - When the app loads, wait for the announcement: "Application ready. Press F5 to start recording."
   - The Start Recording button will have focus

2. **Recording Speech**:
   - Press F5 to start recording
   - You'll hear: "Recording started. Speak in Welsh. Press F6 to stop."
   - Speak clearly in Welsh
   - Partial transcriptions will appear but won't interrupt

3. **Stopping Recording**:
   - Press F6 to stop
   - You'll hear: "Recording stopped. Ready to record again. Press F5 to start."

4. **Reviewing Transcriptions**:
   - Use Tab to navigate to the completed transcriptions area
   - Your screen reader will read all transcribed sentences with timestamps

5. **Clearing Text**:
   - Press F7 at any time to clear all transcriptions
   - You'll hear: "All transcriptions cleared."

## Error Handling

All errors are announced to screen readers with assertive priority, ensuring you're immediately aware of any issues:
- Model loading errors
- Recording device errors
- Network errors during model download

## Future Accessibility Improvements

We're committed to continuous accessibility improvements. Planned enhancements include:
- Configurable announcement verbosity
- Additional keyboard shortcuts
- Customizable audio feedback
- Enhanced bilingual support

## Feedback

If you encounter any accessibility issues or have suggestions for improvements, please:
- Open an issue on GitHub
- Include details about your screen reader and operating system
- Describe the specific accessibility challenge

## Technical Details

### Implementation Files
- UI Definition: [src/MainWindow.axaml](../src/MainWindow.axaml)
- Code Logic: [src/MainWindow.axaml.cs](../src/MainWindow.axaml.cs)

### Key Accessibility Methods
- `AnnounceToScreenReader()` - Manages screen reader announcements
- `Window_KeyDown()` - Handles keyboard shortcuts
- `UpdateStatus()` - Updates status with announcements
- Focus management in Start/Stop recording methods

## Resources

- [Avalonia Automation Documentation](https://docs.avaloniaui.net/docs/concepts/accessibility)
- [WCAG 2.1 Guidelines](https://www.w3.org/WAI/WCAG21/quickref/)
- [ARIA Live Regions](https://developer.mozilla.org/en-US/docs/Web/Accessibility/ARIA/ARIA_Live_Regions)
