using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using Vosk;
using Newtonsoft.Json.Linq;
using OpenTK.Audio.OpenAL;

namespace VoskWelshSpeechRecognitionAvalonia;

public partial class MainWindow : Window
{
    private Model? _model;
    private VoskRecognizer? _recognizer;
    private ALCaptureDevice? _captureDevice;
    private BlockingCollection<byte[]>? _audioQueue;
    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _processingTask;
    private Task? _captureTask;
    private bool _isRecording = false;

    // Audio configuration
    private const int SampleRate = 16000;
    private const int Channels = 1;
    private const int BitsPerSample = 16;
    private const int BufferSize = 4096;

    public MainWindow()
    {
        InitializeComponent();
        InitializeModel();
    }

    private void Window_KeyDown(object? sender, KeyEventArgs e)
    {
        // Handle keyboard shortcuts
        switch (e.Key)
        {
            case Key.F5:
                if (StartButton?.IsEnabled == true)
                {
                    StartButton_Click(sender, new RoutedEventArgs());
                    e.Handled = true;
                }
                break;
            case Key.F6:
                if (StopButton?.IsEnabled == true)
                {
                    StopButton_Click(sender, new RoutedEventArgs());
                    e.Handled = true;
                }
                break;
            case Key.F7:
                ClearButton_Click(sender, new RoutedEventArgs());
                e.Handled = true;
                break;
            case Key.F8:
                CopyWithTimestampsButton_Click(sender, new RoutedEventArgs());
                e.Handled = true;
                break;
            case Key.F9:
                CopyWithoutTimestampsButton_Click(sender, new RoutedEventArgs());
                e.Handled = true;
                break;
        }
    }

    private async void InitializeModel()
    {
        try
        {
            // Disable start button during initialization
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (StartButton != null)
                    StartButton.IsEnabled = false;
            });

            // Store models in user's AppData folder (writable without admin rights)
            string appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string modelsDir = Path.Combine(appDataDir, "Trawsgrifiwr-Byw", "Models");
            string modelPath = Path.Combine(modelsDir, "vosk-model-cy");

            //Console.WriteLine($"Looking for model at: {modelPath}");
            UpdateStatus("Yn paratoi... - Initializing...", Brushes.Orange);

            if (!Directory.Exists(modelPath))
            {
                UpdateStatus("Yn lawrlwytho model iaith Gymraeg... - Downloading Welsh language model...", Brushes.Orange);
                //Console.WriteLine("Model not found, starting download...");

                // Download and extract model
                bool success = await DownloadAndExtractModel(modelsDir);

                if (!success)
                {
                    UpdateStatus("Failed to download model. Please check your internet connection and try restarting the app.", Brushes.Red);
                    //Console.WriteLine("Model download failed!");
                    return;
                }

                //Console.WriteLine("Model downloaded successfully");
            }
            else
            {
                //Console.WriteLine("Model found at expected location");
            }

            UpdateStatus("Loading model into memory...", Brushes.Orange);
            //Console.WriteLine("Creating Vosk Model object...");

            _model = new Model(modelPath);

            //Console.WriteLine("Model loaded successfully!");
            UpdateStatus("Barod i recordio - Ready to record", Brushes.Green);

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (StartButton != null)
                {
                    StartButton.IsEnabled = true;
                    // Set initial focus to start button
                    StartButton.Focus();
                }
                AnnounceToScreenReader("Application ready. Press F5 to start recording.", isAssertive: true);
            });
        }
        catch (Exception ex)
        {
            //Console.WriteLine($"Error in InitializeModel: {ex}");
            UpdateStatus($"Error loading model: {ex.Message}", Brushes.Red);

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (StartButton != null)
                    StartButton.IsEnabled = false;
            });
        }
    }

    private async Task<bool> DownloadAndExtractModel(string modelsDir)
    {
        const string modelUrl = "https://huggingface.co/techiaith/kaldi-cy/resolve/main/model_cy.tar.gz";
        string tarGzPath = "";

        // Store log in AppData (writable without admin rights)
        string appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string logDir = Path.Combine(appDataDir, "Trawsgrifiwr-Byw");
        Directory.CreateDirectory(logDir);
        string logPath = Path.Combine(logDir, "download-error.log");

        LogToFile(logPath, "Starting model download...");
        LogToFile(logPath, $"Download URL: {modelUrl}");
        LogToFile(logPath, $"Target directory: {modelsDir}");

        try
        {
            LogToFile(logPath, $"Creating Models directory: {modelsDir}");
            // Create Models directory if it doesn't exist
            Directory.CreateDirectory(modelsDir);

            if (!Directory.Exists(modelsDir))
            {
                LogToFile(logPath, "FATAL: Failed to create Models directory!");
                UpdateStatus("Error: Failed to create Models directory", Brushes.Red);
                return false;
            }

            LogToFile(logPath, "Models directory created successfully");

            tarGzPath = Path.Combine(modelsDir, "model_cy.tar.gz");
            LogToFile(logPath, $"Will download to: {tarGzPath}");

            // Download the model
            UpdateStatus("Yn lawrlwytho... - Downloading...", Brushes.Orange);
            LogToFile(logPath, "Starting HTTP download...");

            using (var httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromMinutes(30); // Large model, allow time
                LogToFile(logPath, $"HTTP timeout set to 30 minutes");

                LogToFile(logPath, $"Sending GET request to: {modelUrl}");

                using (var response = await httpClient.GetAsync(modelUrl, HttpCompletionOption.ResponseHeadersRead))
                {
                    LogToFile(logPath, $"HTTP Response Status: {response.StatusCode}");
                    LogToFile(logPath, $"HTTP Response Headers: {response.Headers}");

                    response.EnsureSuccessStatusCode();

                    var totalBytes = response.Content.Headers.ContentLength ?? -1L;
                    var canReportProgress = totalBytes != -1;

                    LogToFile(logPath, $"Content length: {totalBytes} bytes ({(totalBytes / 1024.0 / 1024.0):F2}MB)");
                    LogToFile(logPath, $"Can report progress: {canReportProgress}");

                    LogToFile(logPath, "Opening file stream for writing...");
                    using (var contentStream = await response.Content.ReadAsStreamAsync())
                    using (var fileStream = new FileStream(tarGzPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                    {
                        LogToFile(logPath, "File stream opened successfully");
                        var totalRead = 0L;
                        var buffer = new byte[8192];
                        var isMoreToRead = true;

                        do
                        {
                            var read = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                            if (read == 0)
                            {
                                isMoreToRead = false;
                            }
                            else
                            {
                                await fileStream.WriteAsync(buffer, 0, read);
                                totalRead += read;

                                if (canReportProgress)
                                {
                                    var progressPercentage = (double)totalRead / totalBytes * 100;
                                    UpdateStatus($"Downloading: {progressPercentage:F1}% ({totalRead / 1024 / 1024}MB / {totalBytes / 1024 / 1024}MB)", Brushes.Orange);
                                }
                            }
                        }
                        while (isMoreToRead);

                        LogToFile(logPath, $"Download complete. Total bytes read: {totalRead} ({(totalRead / 1024.0 / 1024.0):F2}MB)");
                    }
                }
            }

            LogToFile(logPath, $"Verifying downloaded file exists: {File.Exists(tarGzPath)}");
            if (File.Exists(tarGzPath))
            {
                var fileInfo = new FileInfo(tarGzPath);
                LogToFile(logPath, $"Downloaded file size: {fileInfo.Length} bytes ({(fileInfo.Length / 1024.0 / 1024.0):F2}MB)");
            }

            // Extract the tar.gz file
            UpdateStatus("Extracting model files...", Brushes.Orange);
            LogToFile(logPath, "Starting extraction...");

            await Task.Run(() => ExtractTarGz(tarGzPath, modelsDir, logPath));

            LogToFile(logPath, "Extraction complete");

            // Rename extracted folder to vosk-model-cy
            string extractedPath = Path.Combine(modelsDir, "model");
            string finalPath = Path.Combine(modelsDir, "vosk-model-cy");

            //Console.WriteLine($"Looking for extracted folder at: {extractedPath}");
            //Console.WriteLine($"Extracted folder exists: {Directory.Exists(extractedPath)}");

            if (Directory.Exists(extractedPath))
            {
                //Console.WriteLine($"Renaming to: {finalPath}");
                if (Directory.Exists(finalPath))
                {
                    //Console.WriteLine("Deleting existing final path directory");
                    Directory.Delete(finalPath, true);
                }
                Directory.Move(extractedPath, finalPath);
                //Console.WriteLine("Rename complete");
            }
            else
            {
                //Console.WriteLine("ERROR: Extracted folder not found!");
                UpdateStatus("Error: Extracted folder not found after extraction", Brushes.Red);
                return false;
            }

            // Clean up tar.gz file
            if (File.Exists(tarGzPath))
            {
                //Console.WriteLine("Cleaning up tar.gz file");
                File.Delete(tarGzPath);
            }

            //Console.WriteLine("Model download and extraction completed successfully!");
            UpdateStatus("Model downloaded and extracted successfully!", Brushes.Green);
            return true;
        }
        catch (Exception ex)
        {
            // Log detailed error information
            LogToFile(logPath, $"=== ERROR OCCURRED ===");
            LogToFile(logPath, $"Exception Type: {ex.GetType().FullName}");
            LogToFile(logPath, $"Message: {ex.Message}");
            LogToFile(logPath, $"Stack Trace:\n{ex.StackTrace}");

            if (ex.InnerException != null)
            {
                LogToFile(logPath, $"Inner Exception: {ex.InnerException.GetType().FullName}");
                LogToFile(logPath, $"Inner Message: {ex.InnerException.Message}");
                LogToFile(logPath, $"Inner Stack Trace:\n{ex.InnerException.StackTrace}");
            }

            // Show detailed error in UI
            string errorDetails = $"{ex.GetType().Name}: {ex.Message}";
            if (ex.InnerException != null)
            {
                errorDetails += $"\n\nInner: {ex.InnerException.Message}";
            }

            UpdateStatus($"Download failed. See download-error.log for details.", Brushes.Red);

            // Show detailed error in a message box
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                await ShowErrorDialog("Model Download Failed",
                    $"Failed to download the Welsh language model.\n\n" +
                    $"Error: {errorDetails}\n\n" +
                    $"Detailed logs saved to: download-error.log\n\n" +
                    $"Common causes:\n" +
                    $"• No internet connection\n" +
                    $"• Firewall blocking the download\n" +
                    $"• Antivirus blocking file access\n" +
                    $"• Insufficient disk space\n" +
                    $"• Hugging Face server temporarily unavailable");
            });

            // Clean up partial download
            if (!string.IsNullOrEmpty(tarGzPath) && File.Exists(tarGzPath))
            {
                try
                {
                    File.Delete(tarGzPath);
                    LogToFile(logPath, "Cleaned up partial download");
                }
                catch (Exception cleanupEx)
                {
                    LogToFile(logPath, $"Failed to clean up: {cleanupEx.Message}");
                }
            }

            return false;
        }
    }

    private void ExtractTarGz(string gzArchivePath, string destFolder, string logPath)
    {
        try
        {
            LogToFile(logPath, $"ExtractTarGz called with archive: {gzArchivePath}");
            LogToFile(logPath, $"Destination folder: {destFolder}");

            using (Stream inStream = File.OpenRead(gzArchivePath))
            {
                LogToFile(logPath, "Opened gzip file stream");
                using (Stream gzipStream = new GZipInputStream(inStream))
                {
                    LogToFile(logPath, "Created GZipInputStream");
                    using (TarArchive tarArchive = TarArchive.CreateInputTarArchive(gzipStream, System.Text.Encoding.UTF8))
                    {
                        LogToFile(logPath, "Created TarArchive, starting extraction...");
                        tarArchive.ExtractContents(destFolder);
                        LogToFile(logPath, "TarArchive extraction completed");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            LogToFile(logPath, $"ERROR during extraction: {ex.GetType().Name}");
            LogToFile(logPath, $"Extraction error message: {ex.Message}");
            LogToFile(logPath, $"Extraction stack trace: {ex.StackTrace}");
            throw; // Re-throw to be caught by outer handler
        }
    }

    private void StartButton_Click(object? sender, RoutedEventArgs e)
    {
        if (_model == null)
        {
            return;
        }

        try
        {
            StartRecording();
        }
        catch (Exception ex)
        {
            UpdateStatus($"Error starting recording: {ex.Message}", Brushes.Red);
        }
    }

    private void StopButton_Click(object? sender, RoutedEventArgs e)
    {
        StopRecording();
    }

    private void ClearButton_Click(object? sender, RoutedEventArgs e)
    {
        if (LiveInputText != null)
            LiveInputText.Text = "Partial transcription will appear here... / Bydd trawsgrifiad rhannol yn ymddangos yma...";
        if (TranscriptionsText != null)
            TranscriptionsText.Text = "Completed sentences will appear here... / Bydd brawddegau cyflawn yn ymddangos yma...";

        AnnounceToScreenReader("All transcriptions cleared.", isAssertive: true);
    }

    private async void CopyWithTimestampsButton_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (TranscriptionsText != null)
            {
                string text = TranscriptionsText.Text;

                // Check if there's actual content (not the placeholder text)
                if (!text.Contains("Completed sentences will appear here"))
                {
                    var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
                    if (clipboard != null)
                    {
                        await clipboard.SetTextAsync(text);
                        AnnounceToScreenReader("Transcriptions with timestamps copied to clipboard.", isAssertive: true);

                        // Visual feedback
                        UpdateStatus("Copied to clipboard with timestamps / Wedi copïo i'r clipfwrdd gydag amseroedd", Brushes.Green);

                        // Reset status after 2 seconds
                        await Task.Delay(2000);
                        if (!_isRecording)
                        {
                            UpdateStatus("Barod i recordio - Ready to record", Brushes.Green);
                        }
                    }
                }
                else
                {
                    AnnounceToScreenReader("No transcriptions to copy.", isAssertive: true);
                    UpdateStatus("No transcriptions to copy / Dim trawsgrifiadau i'w copïo", Brushes.Orange);

                    await Task.Delay(2000);
                    if (!_isRecording)
                    {
                        UpdateStatus("Barod i recordio - Ready to record", Brushes.Green);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            AnnounceToScreenReader($"Error copying to clipboard: {ex.Message}", isAssertive: true);
            UpdateStatus($"Error copying to clipboard: {ex.Message}", Brushes.Red);
        }
    }

    private async void CopyWithoutTimestampsButton_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (TranscriptionsText != null)
            {
                string text = TranscriptionsText.Text;

                // Check if there's actual content (not the placeholder text)
                if (!text.Contains("Completed sentences will appear here"))
                {
                    // Remove timestamps from each line
                    var lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    var textOnly = new System.Text.StringBuilder();

                    foreach (var line in lines)
                    {
                        // Remove timestamp pattern [HH:mm:ss] from the beginning
                        var match = System.Text.RegularExpressions.Regex.Match(line, @"^\[\d{2}:\d{2}:\d{2}\]\s*(.*)$");
                        if (match.Success)
                        {
                            // Add the text without timestamp
                            if (textOnly.Length > 0)
                                textOnly.Append(' '); // Add space between segments
                            textOnly.Append(match.Groups[1].Value.Trim());
                        }
                        else
                        {
                            // If no timestamp pattern found, add the line as is
                            if (textOnly.Length > 0)
                                textOnly.Append(' ');
                            textOnly.Append(line.Trim());
                        }
                    }

                    var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
                    if (clipboard != null)
                    {
                        await clipboard.SetTextAsync(textOnly.ToString());
                        AnnounceToScreenReader("Transcriptions without timestamps copied to clipboard as continuous text.", isAssertive: true);

                        // Visual feedback
                        UpdateStatus("Copied to clipboard without timestamps / Wedi copïo i'r clipfwrdd heb amseroedd", Brushes.Green);

                        // Reset status after 2 seconds
                        await Task.Delay(2000);
                        if (!_isRecording)
                        {
                            UpdateStatus("Barod i recordio - Ready to record", Brushes.Green);
                        }
                    }
                }
                else
                {
                    AnnounceToScreenReader("No transcriptions to copy.", isAssertive: true);
                    UpdateStatus("No transcriptions to copy / Dim trawsgrifiadau i'w copïo", Brushes.Orange);

                    await Task.Delay(2000);
                    if (!_isRecording)
                    {
                        UpdateStatus("Barod i recordio - Ready to record", Brushes.Green);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            AnnounceToScreenReader($"Error copying to clipboard: {ex.Message}", isAssertive: true);
            UpdateStatus($"Error copying to clipboard: {ex.Message}", Brushes.Red);
        }
    }

    private void StartRecording()
    {
        if (_isRecording) return;

        // Check if model is loaded
        if (_model == null)
        {
            UpdateStatus("Error: Model not loaded. Cannot start recording.", Brushes.Red);
            return;
        }

        Console.WriteLine("Starting recording...");

        // Initialize recognizer
        _recognizer = new VoskRecognizer(_model, SampleRate);
        _recognizer.SetMaxAlternatives(0);
        _recognizer.SetWords(true);

        // Initialize audio queue and cancellation token
        _audioQueue = new BlockingCollection<byte[]>(boundedCapacity: 100);
        _cancellationTokenSource = new CancellationTokenSource();

        // Start audio processing task
        _processingTask = Task.Run(() => ProcessAudioQueue(_cancellationTokenSource.Token));

        // Open default capture device with OpenAL
        try
        {
            // Get default capture device
            string? deviceName = ALC.GetString(ALDevice.Null, AlcGetString.CaptureDefaultDeviceSpecifier);
            Console.WriteLine($"Using capture device: {deviceName}");

            // Open capture device (16-bit mono at 16kHz)
            _captureDevice = ALC.CaptureOpenDevice(deviceName, SampleRate, ALFormat.Mono16, BufferSize);

            if (_captureDevice == null || _captureDevice == ALCaptureDevice.Null)
            {
                throw new Exception("Failed to open audio capture device");
            }

            // Start capture
            ALC.CaptureStart(_captureDevice.Value);
            Console.WriteLine("Capture started");

            // Start capture task
            _captureTask = Task.Run(() => CaptureAudioLoop(_cancellationTokenSource.Token));

            _isRecording = true;

            // Update UI
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (StartButton != null)
                    StartButton.IsEnabled = false;
                if (StopButton != null)
                {
                    StopButton.IsEnabled = true;
                    // Move focus to stop button for accessibility
                    StopButton.Focus();
                }
                UpdateStatus("Yn recordio... Siaradwch yn Gymraeg - Recording... Speak in Welsh", Brushes.Red);
                AnnounceToScreenReader("Recording started. Speak in Welsh. Press F6 to stop.", isAssertive: true);
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error starting capture: {ex.Message}");
            UpdateStatus($"Error starting recording: {ex.Message}", Brushes.Red);
            AnnounceToScreenReader($"Error starting recording: {ex.Message}", isAssertive: true);
            _isRecording = false;
        }
    }

    private void StopRecording()
    {
        if (!_isRecording) return;

        Console.WriteLine("Stopping recording...");
        _isRecording = false;

        // Signal tasks to stop
        _cancellationTokenSource?.Cancel();

        // Stop audio capture
        if (_captureDevice != null && _captureDevice != ALCaptureDevice.Null)
        {
            try
            {
                ALC.CaptureStop(_captureDevice.Value);
                ALC.CaptureCloseDevice(_captureDevice.Value);
                Console.WriteLine("Capture device closed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error closing capture device: {ex.Message}");
            }
            _captureDevice = null;
        }

        // Wait for tasks to complete
        try
        {
            _captureTask?.Wait(TimeSpan.FromSeconds(2));
            _processingTask?.Wait(TimeSpan.FromSeconds(2));
        }
        catch (AggregateException) { /* Ignore cancellation exceptions */ }

        // Cleanup
        _audioQueue?.Dispose();
        _audioQueue = null;
        _recognizer?.Dispose();
        _recognizer = null;
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;

        // Update UI
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            if (StartButton != null)
            {
                StartButton.IsEnabled = true;
                // Move focus back to start button for accessibility
                StartButton.Focus();
            }
            if (StopButton != null)
                StopButton.IsEnabled = false;

            // Clear partial result when stopping
            if (LiveInputText != null)
                LiveInputText.Text = "Partial transcription will appear here... / Bydd trawsgrifiad rhannol yn ymddangos yma...";

            UpdateStatus("Recordio wedi stopio - Barod i recordio eto / Recording stopped - Ready to record again", Brushes.Orange);
            AnnounceToScreenReader("Recording stopped. Ready to record again. Press F5 to start.", isAssertive: true);
        });
    }

    private void CaptureAudioLoop(CancellationToken cancellationToken)
    {
        if (_captureDevice == null || _captureDevice == ALCaptureDevice.Null)
            return;

        Console.WriteLine("Audio capture loop started");

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                // Check how many samples are available
                int samplesAvailable = ALC.GetAvailableSamples(_captureDevice.Value);

                if (samplesAvailable > 0)
                {
                    // Read samples (2 bytes per sample for 16-bit audio)
                    short[] samples = new short[samplesAvailable];
                    ALC.CaptureSamples(_captureDevice.Value, samples, samplesAvailable);

                    // Convert to byte array
                    byte[] audioData = new byte[samplesAvailable * 2];
                    Buffer.BlockCopy(samples, 0, audioData, 0, audioData.Length);

                    // Add to queue for processing
                    if (_audioQueue != null && !_audioQueue.IsAddingCompleted)
                    {
                        try
                        {
                            _audioQueue.TryAdd(audioData, 10);  // 10ms timeout
                        }
                        catch (InvalidOperationException) { /* Queue disposed */ }
                    }
                }

                // Sleep briefly to avoid busy waiting
                Thread.Sleep(10);
            }

            Console.WriteLine("Audio capture loop ended");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in capture loop: {ex.Message}");
            Dispatcher.UIThread.Post(() =>
            {
                UpdateStatus($"Recording error: {ex.Message}", Brushes.Red);
            });
        }
        finally
        {
            // Mark audio queue as complete
            _audioQueue?.CompleteAdding();
        }
    }

    private void ProcessAudioQueue(CancellationToken cancellationToken)
    {
        if (_audioQueue == null || _recognizer == null) return;

        try
        {
            foreach (var audioData in _audioQueue.GetConsumingEnumerable(cancellationToken))
            {
                if (cancellationToken.IsCancellationRequested) break;

                // Process audio with Vosk
                bool isFinalResult = _recognizer.AcceptWaveform(audioData, audioData.Length);

                if (isFinalResult)
                {
                    // Final result - utterance complete
                    string resultJson = _recognizer.Result();
                    Console.WriteLine($"Final result detected: {resultJson}");
                    ProcessFinalResult(resultJson);
                }
                else
                {
                    // Partial result - still speaking
                    string partialJson = _recognizer.PartialResult();
                    ProcessPartialResult(partialJson);
                }
            }

            // Get any remaining final result when stopping
            if (_recognizer != null)
            {
                string finalJson = _recognizer.FinalResult();
                ProcessFinalResult(finalJson);
            }
        }
        catch (OperationCanceledException) { /* Expected when stopping */ }
        catch (Exception ex)
        {
            Dispatcher.UIThread.Post(() =>
            {
                UpdateStatus($"Processing error: {ex.Message}", Brushes.Red);
            });
        }
    }

    private void ProcessPartialResult(string jsonResult)
    {
        try
        {
            var result = JObject.Parse(jsonResult);
            string partialText = result["partial"]?.ToString() ?? "";

            Dispatcher.UIThread.Post(() =>
            {
                if (!string.IsNullOrWhiteSpace(partialText) && LiveInputText != null)
                {
                    LiveInputText.Text = partialText;
                }
            });
        }
        catch (Exception ex)
        {
            //Console.WriteLine($"Error parsing partial result: {ex.Message}");
        }
    }

    private async void ProcessFinalResult(string jsonResult)
    {
        try
        {
            Console.WriteLine($"ProcessFinalResult called with: {jsonResult}");
            var result = JObject.Parse(jsonResult);
            string text = result["text"]?.ToString()?.Trim() ?? "";

            Console.WriteLine($"Extracted text: '{text}'");

            if (!string.IsNullOrWhiteSpace(text))
            {
                string timestamp = DateTime.Now.ToString("HH:mm:ss");
                string formattedResult = $"[{timestamp}] {text}";

                Console.WriteLine($"Adding to completed transcriptions: {formattedResult}");

                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    System.Diagnostics.Debug.WriteLine($"UI Thread: LiveInputText is null? {LiveInputText == null}");
                    System.Diagnostics.Debug.WriteLine($"UI Thread: TranscriptionsText is null? {TranscriptionsText == null}");

                    // Clear live input
                    if (LiveInputText != null)
                        LiveInputText.Text = "Partial transcription will appear here... / Bydd trawsgrifiad rhannol yn ymddangos yma...";

                    // Add to transcriptions
                    if (TranscriptionsText != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"Current TranscriptionsText.Text: '{TranscriptionsText.Text}'");

                        if (TranscriptionsText.Text.Contains("Completed sentences will appear here"))
                        {
                            System.Diagnostics.Debug.WriteLine("Replacing default text with first result");
                            TranscriptionsText.Text = formattedResult;
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Appending to existing text");
                            TranscriptionsText.Text += Environment.NewLine + formattedResult;
                        }

                        System.Diagnostics.Debug.WriteLine($"New TranscriptionsText.Text: '{TranscriptionsText.Text}'");

                        // Force UI update
                        TranscriptionsText.InvalidateVisual();

                        // Auto-scroll to bottom
                        TranscriptionsScrollViewer?.ScrollToEnd();

                        // Announce the completed transcription to screen readers
                        AnnounceToScreenReader($"Transcribed: {text}", isAssertive: false);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("ERROR: TranscriptionsText is null!");
                    }
                });
            }
            else
            {
                Console.WriteLine("Text was empty or whitespace, not adding to transcriptions");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing final result: {ex.Message}");
        }
    }

    private void UpdateStatus(string message, IBrush color)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (StatusText != null)
            {
                StatusText.Text = message;
                // Announce status changes to screen readers
                AnnounceToScreenReader(message, isAssertive: true);
            }

            if (StatusIndicator != null)
                StatusIndicator.Fill = color;

            Console.WriteLine($"Status updated: {message}");
        });
    }

    private void AnnounceToScreenReader(string message, bool isAssertive = false)
    {
        // Create a temporary TextBlock for screen reader announcements
        // This ensures the announcement happens even if the main text hasn't changed
        var announcement = new TextBlock
        {
            Text = message,
            IsVisible = false
        };

        AutomationProperties.SetLiveSetting(announcement, isAssertive ? AutomationLiveSetting.Assertive : AutomationLiveSetting.Polite);
        AutomationProperties.SetName(announcement, message);

        // Add to main grid temporarily
        if (this.Content is Grid mainGrid)
        {
            mainGrid.Children.Add(announcement);

            // Remove after a short delay
            Dispatcher.UIThread.Post(() =>
            {
                mainGrid.Children.Remove(announcement);
            }, DispatcherPriority.Background);
        }
    }

    private void LogToFile(string logPath, string message)
    {
        try
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string logMessage = $"[{timestamp}] {message}\n";
            File.AppendAllText(logPath, logMessage);
        }
        catch
        {
            // If logging fails, don't crash the app
        }
    }

    private async Task ShowErrorDialog(string title, string message)
    {
        // Create a window to show error details
        var errorWindow = new Window
        {
            Title = title,
            Width = 600,
            Height = 450,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = true
        };

        var scrollViewer = new ScrollViewer
        {
            Padding = new Avalonia.Thickness(20),
            HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
            VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto
        };

        var stackPanel = new StackPanel
        {
            Spacing = 15
        };

        // Error message
        var messageText = new TextBlock
        {
            Text = message,
            TextWrapping = Avalonia.Media.TextWrapping.Wrap,
            FontSize = 14
        };

        // Log file location
        string appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string logPath = Path.Combine(appDataDir, "Trawsgrifiwr-Byw", "download-error.log");

        var logLocationText = new TextBlock
        {
            Text = $"\nLog file location:\n{logPath}",
            TextWrapping = Avalonia.Media.TextWrapping.Wrap,
            FontSize = 12,
            Foreground = Brushes.Gray,
            Margin = new Avalonia.Thickness(0, 10, 0, 0)
        };

        // Buttons
        var buttonPanel = new StackPanel
        {
            Orientation = Avalonia.Layout.Orientation.Horizontal,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            Spacing = 10,
            Margin = new Avalonia.Thickness(0, 20, 0, 0)
        };

        var openLogButton = new Button
        {
            Content = "Open Log File",
            Padding = new Avalonia.Thickness(15, 8)
        };
        openLogButton.Click += (s, e) =>
        {
            try
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string logFile = Path.Combine(appData, "Trawsgrifiwr-Byw", "download-error.log");
                if (File.Exists(logFile))
                {
                    // Open log file with default text editor
                    if (OperatingSystem.IsWindows())
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = logFile,
                            UseShellExecute = true
                        });
                    }
                    else if (OperatingSystem.IsMacOS())
                    {
                        System.Diagnostics.Process.Start("open", logFile);
                    }
                    else if (OperatingSystem.IsLinux())
                    {
                        System.Diagnostics.Process.Start("xdg-open", logFile);
                    }
                }
            }
            catch
            {
                // If opening fails, ignore
            }
        };

        var okButton = new Button
        {
            Content = "OK",
            Padding = new Avalonia.Thickness(15, 8)
        };
        okButton.Click += (s, e) => errorWindow.Close();

        buttonPanel.Children.Add(openLogButton);
        buttonPanel.Children.Add(okButton);

        stackPanel.Children.Add(messageText);
        stackPanel.Children.Add(logLocationText);
        stackPanel.Children.Add(buttonPanel);

        scrollViewer.Content = stackPanel;
        errorWindow.Content = scrollViewer;

        await errorWindow.ShowDialog(this);
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        StopRecording();
        _model?.Dispose();
        base.OnClosing(e);
    }
}