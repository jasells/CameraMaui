using Camera.MAUI.Ex.Models;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Camera.MAUI.Ex.Resources;
using Camera.MAUI;
using CommunityToolkit.Mvvm.Input;
using Colors = Microsoft.Maui.Graphics.Colors;

namespace Camera.MAUI.Ex.Services;

/// <summary>
/// Used to manage the internal states of the VideoRecordView to allow easier access to app-code.
/// declared as partial to support use of <see cref="CommunityToolkit.Mvvm.Input.RelayCommand"/>
/// </summary>
public partial class VideoRecordingService : NotifyPropertyChangedBase, IVideoRecordingService
{
    public string OngoingRecordingMessage
    {
        get => _ongoingRecordingMessage;
        set => SetProperty(ref _ongoingRecordingMessage, value);
    }
    private string _ongoingRecordingMessage = "Video recording in progress.";
    
    public string StartRecordingMessage
    {
        get => _startRecordingMessage;
        set => SetProperty(ref _startRecordingMessage, value);
    }
    private string _startRecordingMessage = "Hit the \"record\" button to start your video.";
    
    public Color RecordingMessageTextColor
    {
        get => _recordingMessageTextColor;
        set => SetProperty(ref _recordingMessageTextColor, value);
    }
    private Color _recordingMessageTextColor = Color.FromArgb("#FFFFFF");
    
    public Color RecordingMessageBackgroundColor
    {
        get => _recordingMessageBackgroundColor;
        set => SetProperty(ref _recordingMessageBackgroundColor, value);
    }
    private Color _recordingMessageBackgroundColor = Color.FromArgb("#80000000");
    
    public double RecordingMessageFontSize
    {
        get => _recordingMessageFontSize;
        set => SetProperty(ref _recordingMessageFontSize, value);
    }
    private double _recordingMessageFontSize = 14d;
    
    public Thickness RecordingMessagePadding
    {
        get => _recordingMessagePadding;
        set => SetProperty(ref _recordingMessagePadding, value);
    }
    private Thickness _recordingMessagePadding = new Thickness(20,10,20,10);
    
    public double TimerFontSize
    {
        get => _timerFontSize;
        set => SetProperty(ref _timerFontSize, value);
    }
    private double _timerFontSize = 14d;
    
    public Color HeaderBackgroundColor
    {
        get => _headerBackgroundColor;
        set => SetProperty(ref _headerBackgroundColor, value);
    }
    private Color _headerBackgroundColor = Camera.MAUI.Ex.Resources.Colors.Leaf;
    
    public Color RecordViewBackgroundColor
    {
        get => _recordViewBackgroundColor;
        set => SetProperty(ref _recordViewBackgroundColor, value);
    }
    private Color _recordViewBackgroundColor = Camera.MAUI.Ex.Resources.Colors.Snow;
    
    public Color ProgressColor
    {
        get => _progressColor;
        set => SetProperty(ref _progressColor, value);
    }
    private Color _progressColor = Camera.MAUI.Ex.Resources.Colors.Snow;
    
    public double ProgressHeight
    {
        get => _progressHeight;
        set => SetProperty(ref _progressHeight, value);
    }
    private double _progressHeight = 20d;
    
    public Color TimerDefaultTextColor
    {
        get => _timerDefaultTextColor;
        set
        {
            SetProperty(ref _timerDefaultTextColor, value);
            OnPropertyChanged(nameof(TimerTextColor));
        }
    }

    private Color _timerDefaultTextColor = Camera.MAUI.Ex.Resources.Colors.Snow;
    
    public Color TimerDefaultBackgroundColor
    {
        get => _timerDefaultBackgroundColor;
        set
        {
            SetProperty(ref _timerDefaultBackgroundColor, value);
            OnPropertyChanged(nameof(TimerBackgroundColor));
        }
    }

    private Color _timerDefaultBackgroundColor = Colors.Transparent;
    
    public Color TimerEndingTextColor
    {
        get => _timerEndingTextColor;
        set => SetProperty(ref _timerEndingTextColor, value);
    }
    private Color _timerEndingTextColor = Camera.MAUI.Ex.Resources.Colors.Hibiscus;
    
    public Color TimerEndingBackgroundColor
    {
        get => _timerEndingBackgroundColor;
        set => SetProperty(ref _timerEndingBackgroundColor, value);
    }
    private Color _timerEndingBackgroundColor = Camera.MAUI.Ex.Resources.Colors.Snow;
    
    public Thickness TimerLabelPadding
    {
        get => _timerPadding;
        set => SetProperty(ref _timerPadding, value);
    }
    private Thickness _timerPadding = new Thickness(4,2,4,2);
    
    public string SecondsRemainingFormatted => TimeSpan.FromSeconds(SecondsRemaining).ToString(@"mm\:ss");

    public double Progress
    {
        get => _progress;
        private set => SetProperty(ref _progress, value);
    }
    private double _progress = 1d;
    
    public Color TimerTextColor => SecondsRemaining <= AlertTimeRemaining ?
                                                        TimerEndingTextColor : TimerDefaultTextColor;
    
    public Color TimerBackgroundColor => SecondsRemaining <= AlertTimeRemaining ?
                                                        TimerEndingBackgroundColor : TimerDefaultBackgroundColor;
    
    public Thickness CountdownTimerViewPadding
    {
        get => _countdownTimerViewPadding;
        set => SetProperty(ref _countdownTimerViewPadding, value);
    }
    private Thickness _countdownTimerViewPadding = new Thickness(20,10, 20, 10);
    
    public double CountdownTimerViewSpacing
    {
        get => _countdownTimerViewSpacing;
        set => SetProperty(ref _countdownTimerViewSpacing, value);
    }
    private double _countdownTimerViewSpacing = 20d;
    
    public double RecordButtonSize
    {
        get => _recordButtonSize;
        set
        {
            SetProperty(ref _recordButtonSize, value);
            OnPropertyChanged(nameof(PlayIconSize));
            OnPropertyChanged(nameof(StopIconSize));
        }
    }
    private double _recordButtonSize = 72d;
    
    public double PlayIconSize => RecordButtonSize * 0.80;
    public double StopIconSize => RecordButtonSize * 0.40;
    
    public Color RecordButtonOutlineColor
    {
        get => _recordButtonOutlineColor;
        set => SetProperty(ref _recordButtonOutlineColor, value);
    }
    private Color _recordButtonOutlineColor = Camera.MAUI.Ex.Resources.Colors.Hibiscus;
    
    public double RecordButtonOutlineWidth
    {
        get => _recordButtonOutlineWidth;
        set => SetProperty(ref _recordButtonOutlineWidth, value);
    }
    private double _recordButtonOutlineWidth = 2d;
    
    public Color PlayIconColor
    {
        get => _playIconColor;
        set => SetProperty(ref _playIconColor, value);
    }
    private Color _playIconColor = Camera.MAUI.Ex.Resources.Colors.Hibiscus;
    
    public Color StopIconColor
    {
        get => _stopIconColor;
        set => SetProperty(ref _stopIconColor, value);
    }
    private Color _stopIconColor = Camera.MAUI.Ex.Resources.Colors.Hibiscus;
    
    public Thickness RecordButtonMargin
    {
        get => _recordButtonMargin;
        set => SetProperty(ref _recordButtonMargin, value);
    }
    private Thickness _recordButtonMargin = new Thickness(0,0,0,40);
    
    public double StartRecordingBottomSpacing
    {
        get => _startRecordingBottomSpacing;
        set => SetProperty(ref _startRecordingBottomSpacing, value);
    }
    private double _startRecordingBottomSpacing = 10d;
    
    public string TimerFontFamily
    {
        get => _timerFontFamily;
        set => SetProperty(ref _timerFontFamily, value);
    }
    private string _timerFontFamily = FontNames.PromptRegular;
    
    public string RecordingMessageFontFamily
    {
        get => _recordingMessageFontFamily;
        set => SetProperty(ref _recordingMessageFontFamily, value);
    }
    private string _recordingMessageFontFamily = FontNames.PromptRegular;
    
    /// <summary>
    /// Represents the file path as a nullable string. It allows getting and setting the file path while notifying
    /// property changes.
    /// </summary>
    public string? FilePath
    {
        get => _filePath;
        set => SetProperty(ref _filePath, value);
    }
    private string? _filePath = string.Empty;

    public bool RecordVideo
    {
        get => _recordVideo;
        set => SetProperty(ref _recordVideo, value);
    }
    private bool _recordVideo = false;
    
    public double MaxSeconds
    {
        get => _maxSeconds;
        set
        {
            SetProperty(ref _maxSeconds, value);

            CalculateProgress(SecondsRemaining);
        }
    }

    private double _maxSeconds = 60.0; // seconds
    
    public int AlertTimeRemaining
    {
        get => _alertTimeRemaining;
        set
        {
            SetProperty(ref _alertTimeRemaining, value);
            
            OnPropertyChanged(nameof(TimerTextColor));
            OnPropertyChanged(nameof(TimerBackgroundColor));
        }
    }

    private int _alertTimeRemaining = 10; // seconds

    public double SecondsRemaining
    {
        get => _secondsRemaining;
        set
        {
            if (SetProperty(ref _secondsRemaining, value > 0 ? value : 0))
            {
                if (_currentState == VideoRecordState.Recording && _secondsRemaining <= 0)
                {
                    Dispatcher?.DispatchAsync(() => StopRecordingInternal());
                }
            }
            
            OnPropertyChanged(nameof(SecondsRemainingFormatted));
            OnPropertyChanged(nameof(TimerTextColor));
            OnPropertyChanged(nameof(TimerBackgroundColor));
            CalculateProgress(_secondsRemaining);
        }
    }
    private double _secondsRemaining =  60.0; // seconds

    public bool RecordButtonEnabled
    {
        get => _recordButtonEnabled;
        set
        {
            SetProperty(ref _recordButtonEnabled, value);
            Debug.WriteLine($"RecordButtonEnabled: {value}");
        }
    }
    private bool _recordButtonEnabled = false;

    public VideoRecordState CurrentState
    {
        get => _currentState;
        set
        {
            SetProperty(ref _currentState, value);
            
            RecordVideo = CurrentState == VideoRecordState.Recording;

            if (CurrentState != VideoRecordState.Recording)
            {
                SecondsRemaining = MaxSeconds; // Reset seconds remaining when not recording
            }
        }
    }
    private VideoRecordState _currentState = VideoRecordState.Preview;

    public IAsyncRelayCommand RecordingComplete
    { get => _recordingComplete; set => SetProperty(ref _recordingComplete, value); }
    private IAsyncRelayCommand _recordingComplete;


    public IAsyncRelayCommand<string> StartRecordingCommand
    { get => _startRecordingCommand; set => SetProperty(ref _startRecordingCommand, value); }
    private IAsyncRelayCommand<string> _startRecordingCommand;


    public IDispatcher? Dispatcher 
    {
        get => _dispatcher;
        set => SetProperty(ref _dispatcher, value); 
    }
    private IDispatcher? _dispatcher;

    public IAsyncRelayCommand ToggleVideoCommand
    {
        get => _toggleVideoCommand;
        set => SetProperty(ref _toggleVideoCommand, value);
    }
    private IAsyncRelayCommand _toggleVideoCommand;

    public VideoRecordingService(ICameraService camService)
    {
        SecondsRemaining = MaxSeconds; // Default to 60 seconds
        _cameraService = camService;

        SetupRecordTimer();
        
        // Subscribe to camera changes to update RecordButtonEnabled
        _cameraService.CameraChanged += (s, camera) => RecordButtonEnabled = camera != null;

        StartRecordingCommand = new AsyncRelayCommand<string>(StartRecordingAsync,
                                                              AsyncRelayCommandOptions
                                                                .FlowExceptionsToTaskScheduler);

        ToggleVideoCommand = new AsyncRelayCommand(ToggleVideo, CanToggleRecord);
    }

    public async Task StopRecordingInternal(bool callRecordingCompleteCmd = true)
    {
        if (CurrentState == VideoRecordState.Recording)
        {
            countdownTimer.Stop();

            var result = await _cameraService.StopRecordingAsync();
            CurrentState = VideoRecordState.Preview;
            Debug.WriteLine($"stop record result: {result}");

            if (callRecordingCompleteCmd) await RecordingComplete?.ExecuteAsync(null);

            RecordButtonEnabled = true;
        }
    }

    protected virtual async Task<CameraResult> StartRecordingAsync(string file)
    {
        var result = await _cameraService.StartRecordingAsync(file).ConfigureAwait(false);
        Debug.WriteLine($"start recording: {result}");
        return result;
    }

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        Debug.WriteLine($"===== {nameof(VideoRecordingService)}.{propertyName} changed");
    }

    private void CalculateProgress(double secondsRemaining)
    {
        Progress = secondsRemaining / MaxSeconds;
    }

    private void SetupRecordTimer()
    {
        countdownTimer.Interval = 1000; // 1 second interval
        countdownTimer.AutoReset = true;

        countdownTimer.Elapsed += (s, e) =>
        {
            SecondsRemaining--;
        };
    }

    private bool CanToggleRecord() => RecordButtonEnabled;

    private async System.Threading.Tasks.Task ToggleVideo()
    {
        if (CurrentState == VideoRecordState.CamOff)
        {
            bool current = _cameraService.CameraView.AutoStartPreview;
            _cameraService.CameraView.AutoStartPreview = true;
            CurrentState = VideoRecordState.Preview;
        }
        else if (CurrentState == VideoRecordState.Preview)
        {
            var path = Path.Combine(FileSystem.CacheDirectory, $"{Guid.NewGuid()}.mp4");
            FilePath = path;
            File.Create(path);

            SecondsRemaining = MaxSeconds; // Reset seconds remaining to 60 seconds
            // saving current state.
            CurrentState = VideoRecordState.Recording;

            if (StartRecordingCommand != null && StartRecordingCommand.CanExecute(FilePath))
            {
                try
                {
                    // If app-code tries to override this command incorrectly, 
                    // this seg-faults on Android... can't even catch the exception...
                    await StartRecordingCommand.ExecuteAsync(FilePath!);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error starting recording: {ex}");
                    CurrentState = VideoRecordState.Preview;
                    return;
                }
            }

            countdownTimer.Start();
        }
        else
        {
            await StopRecordingInternal().ConfigureAwait(false);
        }
    }

    private readonly ICameraService _cameraService;
    private System.Timers.Timer countdownTimer = new System.Timers.Timer();
}