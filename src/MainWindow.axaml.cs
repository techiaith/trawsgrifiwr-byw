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

            // Look for model in Models subdirectory
            string modelsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Models");
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

        //Console.WriteLine("Starting model download...");

        try
        {
            //Console.WriteLine($"Creating Models directory: {modelsDir}");
            // Create Models directory if it doesn't exist
            Directory.CreateDirectory(modelsDir);

            if (!Directory.Exists(modelsDir))
            {
                //Console.WriteLine("Failed to create Models directory!");
                return false;
            }

            tarGzPath = Path.Combine(modelsDir, "model_cy.tar.gz");
            //Console.WriteLine($"Will download to: {tarGzPath}");

            // Download the model
            UpdateStatus("Yn lawrlwytho... - Downloading...", Brushes.Orange);

            using (var httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromMinutes(30); // Large model, allow time

                //Console.WriteLine($"Starting HTTP download from: {modelUrl}");

                using (var response = await httpClient.GetAsync(modelUrl, HttpCompletionOption.ResponseHeadersRead))
                {
                    //Console.WriteLine($"HTTP Response Status: {response.StatusCode}");
                    response.EnsureSuccessStatusCode();

                    var totalBytes = response.Content.Headers.ContentLength ?? -1L;
                    var canReportProgress = totalBytes != -1;

                    //Console.WriteLine($"Content length: {totalBytes} bytes ({totalBytes / 1024 / 1024}MB)");

                    using (var contentStream = await response.Content.ReadAsStreamAsync())
                    using (var fileStream = new FileStream(tarGzPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                    {
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

                        //Console.WriteLine($"Download complete. Total bytes read: {totalRead}");
                    }
                }
            }

            //Console.WriteLine($"Verifying downloaded file exists: {File.Exists(tarGzPath)}");

            // Extract the tar.gz file
            UpdateStatus("Extracting model files...", Brushes.Orange);
            //Console.WriteLine("Starting extraction...");

            await Task.Run(() => ExtractTarGz(tarGzPath, modelsDir));

            //Console.WriteLine("Extraction complete");

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
            //Console.WriteLine($"Exception during download: {ex.GetType().Name}");
            //Console.WriteLine($"Message: {ex.Message}");
            //Console.WriteLine($"Stack trace: {ex.StackTrace}");

            UpdateStatus($"Download error: {ex.Message}", Brushes.Red);

            // Clean up partial download
            if (!string.IsNullOrEmpty(tarGzPath) && File.Exists(tarGzPath))
            {
                try
                {
                    File.Delete(tarGzPath);
                    //Console.WriteLine("Cleaned up partial download");
                }
                catch { }
            }

            return false;
        }
    }

    private void ExtractTarGz(string gzArchivePath, string destFolder)
    {
        using (Stream inStream = File.OpenRead(gzArchivePath))
        using (Stream gzipStream = new GZipInputStream(inStream))
        using (TarArchive tarArchive = TarArchive.CreateInputTarArchive(gzipStream, System.Text.Encoding.UTF8))
        {
            tarArchive.ExtractContents(destFolder);
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

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        StopRecording();
        _model?.Dispose();
        base.OnClosing(e);
    }
}