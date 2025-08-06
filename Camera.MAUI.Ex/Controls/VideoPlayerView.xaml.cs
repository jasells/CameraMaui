using System.Diagnostics;
using Camera.MAUI.Ex.Extensions;
using LibVLCSharp.Shared;
using Camera.MAUI.Ex.Models;
using Camera.MAUI.Ex.Services;

namespace Camera.MAUI.Ex.Controls;

public partial class VideoPlayerView
{
    public LibVLCSharp.Shared.MediaPlayer MediaPlayer => _videoService?.CurrentPlayer;

    public IVideoService VideoService
    {
        get => _videoService;
        protected set
        {
            if (_videoService == value) return;
         
            OnPropertyChanging(nameof(VideoService));
            _videoService = value;
            OnPropertyChanged(nameof(VideoService));
        }
    }
    private IVideoService _videoService;
    
    public ICameraService CameraService
    {
        get => _camService;
        private set
        {
            _camService = value;
            OnPropertyChanged();
        }
    }
    private ICameraService _camService;
    
    public static readonly BindableProperty IsVideoPreviewProperty = BindableProperty.Create(
        nameof(IsVideoPreview),
        typeof(bool),
        typeof(VideoPlayerView),
        true);
    
    public bool IsVideoPreview
    {
        get => (bool)GetValue(IsVideoPreviewProperty);
        set => SetValue(IsVideoPreviewProperty, value);
    }
    
    public static readonly BindableProperty ProgressVideoTimeProperty = BindableProperty.Create(
        nameof(ProgressVideoTime),
        typeof(string),
        typeof(VideoPlayerView),
        "");
    
    public string ProgressVideoTime
    {
        get => (string)GetValue(ProgressVideoTimeProperty);
        set => SetValue(ProgressVideoTimeProperty, value);
    }
    
    public static readonly BindableProperty IsPlayControlVisibleProperty = BindableProperty.Create(
        nameof(IsPlayControlVisible),
        typeof(bool),
        typeof(VideoPlayerView),
        false,
        propertyChanged: OnIsPlayControlVisibleChanged);
    
    public bool IsPlayControlVisible
    {
        get => (bool)GetValue(IsPlayControlVisibleProperty);
        set => SetValue(IsPlayControlVisibleProperty, value);
    }
    
    private bool _isTimerRunning;
    
    public VideoPlayerView()
    {
        InitializeComponent();

        StartHideControlsTimer();
    }

    public void InjectDependencies(IVideoService vidSrv, ICameraService camService)
    {
        // make sure bindings are updated
        VideoService = vidSrv;
        CameraService = camService;

        _videoService.PropertyChanged += videoService_PropertyChanged;
        _videoService.PropertyChanging += videoService_PropertyChanging;
        // don't know why can't access the member VideoView directly to just set it...
        OnPropertyChanged(nameof(MediaPlayer));
    }

    private void videoService_PropertyChanging(object sender, System.ComponentModel.PropertyChangingEventArgs e)
    {
        if (e.PropertyName == nameof(_videoService.CurrentPlayer))
        {
            OnMediaPlayerChanged(this, _videoService.CurrentPlayer, null);
        }
    }

    private void videoService_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_videoService.CurrentPlayer))
        {
            OnMediaPlayerChanged(this, null, _videoService.CurrentPlayer);
        }
        
        if (e.PropertyName == nameof(_videoService.FilePath))
        {
            FilePathPropertyChanged(this, null, _videoService.FilePath);
        }
    }

    private void StartHideControlsTimer()
    {
        _isTimerRunning = true;
        Debug.WriteLine("Timer started. Control will hide in 5 seconds.");

        Dispatcher.DispatchDelayed(TimeSpan.FromSeconds(5), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (!_isTimerRunning) return;
                IsPlayControlVisible = false;
                _isTimerRunning = false;
                Debug.WriteLine("Controls hidden.");
            });
        });
    }
    
    private static void FilePathPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is VideoPlayerView videoPlayerView && newValue is string newFilePath)
        {
            videoPlayerView._videoService?
                           .CreateMediaPlayer(newFilePath,
                                   true,
                                   videoPlayerView.CameraService?.CurrentOrientation ?? CameraOrientation.Portrait);
        }
    }

    private static void OnMediaPlayerChanged(BindableObject bindable, object oldValue, object newValue)
    {
        Debug.WriteLine($"===== VideoPlayerView.{nameof(OnMediaPlayerChanged)}");
        if (bindable is VideoPlayerView view)
        {
            if (oldValue is LibVLCSharp.Shared.MediaPlayer oldPlayer)
            {
                oldPlayer.Playing -= view.MediaPlayer_Playing;
                oldPlayer.Paused -= view.MediaPlayer_Paused;
                oldPlayer.TimeChanged -= view.MediaPlayer_TimeChanged;
            }

            if (newValue is LibVLCSharp.Shared.MediaPlayer newPlayer)
            {
                newPlayer.Playing += view.MediaPlayer_Playing;
                newPlayer.Paused += view.MediaPlayer_Paused;
                newPlayer.TimeChanged += view.MediaPlayer_TimeChanged;
                // don't know why can't access the member VideoView directly to just set it...
                view.OnPropertyChanged(nameof(view.MediaPlayer));
            }
        }
    }

    private static void OnIsPlayControlVisibleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is VideoPlayerView view)
        {
            if ((bool)newValue)
            {
                view.StartHideControlsTimer();
            }
        }
    }

    private void MediaPlayer_TimeChanged(object? sender, MediaPlayerTimeChangedEventArgs e)
    {
        UpdateProgressVideoTime();
    }

    private void MediaPlayer_Playing(object sender, EventArgs e)
    {
        IsPlayControlVisible = true;
        OnPropertyChanged(nameof(MediaPlayer));
    }

    private void MediaPlayer_Paused(object sender, EventArgs e)
    {
        IsPlayControlVisible = true;
        OnPropertyChanged(nameof(MediaPlayer));
    }

    private void UpdateProgressVideoTime()
    {
        if (MediaPlayer == null) return;

        var positionFormatted = DateTimeHelper.FormatTime(MediaPlayer.Time);
        var durationFormatted = DateTimeHelper.FormatTime(MediaPlayer.Length);
        ProgressVideoTime = $"{positionFormatted} / {durationFormatted}";
    }
}