using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using LibVLCSharp.Shared;
using SkiaSharp;
using Camera.MAUI.Ex.Models;
using Camera.MAUI.Ex.Resources;
// MediaPlayer has a conflict with another namespace
using VlcMediaPlayer = LibVLCSharp.Shared.MediaPlayer;
using Microsoft.Maui.Devices;

namespace Camera.MAUI.Ex.Services;

/// <summary>
/// This is mainly used for managing vlc playback state and other vlc functions.
/// </summary>
/// partial is needed b/c of the stupid code-gen from RelayCommand below.  Need to remove that.
public class VideoService : NotifyPropertyChangedBase, IVideoService
{
    public event EventHandler<string> ThumbnailGenerated;
    public event EventHandler<double> ThumbnailWidthChanged;

    #region bindable props
    public IAsyncRelayCommand BackCommand
    {
        get => _backCommand;
        set => SetProperty(ref _backCommand, value);
    }
    private IAsyncRelayCommand _backCommand;

    public ICommand PlayCommand
    {
        get => _playCommand;
        set => SetProperty(ref _playCommand, value);
    }
    private ICommand _playCommand;

    public IAsyncRelayCommand NextCommand
    {
        get => _nextCommand;
        set => SetProperty(ref _nextCommand, value);
    }
    private IAsyncRelayCommand _nextCommand;

    public CameraOrientation VideoOrientation
    {
        get => _videoOrientation;
        set => SetProperty(ref _videoOrientation, value);
    }
    private CameraOrientation _videoOrientation;

    public Size VideoResolution
    {
        get => _videoResolution;
        set => SetProperty(ref _videoResolution, value);
    }
    private Size _videoResolution;
    
    /// <summary>
    /// Down-samples the position events to avoid too many updates to the UI.
    /// </summary>
    public IObservable<float> PositionEvents => _positionEvents.AsObservable();

    // is this even needed if we can inject this service directly into the sliders as needed?
    public Command<double> SeekPlayer
    {
        get => _seekCmd;
        protected set => SetProperty(ref _seekCmd, value);
    }
    private Command<double> _seekCmd;

    public double ThumbnailWidth
    {
        get => _thumbnailWidth;
        set
        {
            if (SetProperty(ref _thumbnailWidth, value, nameof(ThumbnailWidth)))
            {
                ThumbnailWidthChanged?.Invoke(this, _thumbnailWidth);
            }
        }
    }
    double _thumbnailWidth;

    public LibVLCSharp.Shared.MediaPlayer CurrentPlayer
    {
        get => _player;
        set => SetProperty(ref _player, value);
    }
    private LibVLCSharp.Shared.MediaPlayer _player;

    public double VideoViewHeight
    {
        get => _videoViewHeight;
        set => SetProperty(ref _videoViewHeight, value);
    }
    private double _videoViewHeight = 480d;

    public double VideoViewWidth
    {
        get => _videoViewWidth;
        set => SetProperty(ref _videoViewWidth, value);
    }
    private double _videoViewWidth = 640d;

    public string FilePath
    {
        get => _path;
        set => SetProperty(ref _path, value);
    }
    private string _path;

    public Size? VideoSize
    {
        get => _videoSize;
        set => SetProperty(ref _videoSize, value);
    }
    private Size? _videoSize;
    #endregion

    // this region can easily be transplanted to a service dedicated to the UI layout/sizing, if we want.
    #region UI (VidepPlayerView) layout properties
    /// <summary>
    /// Used to control the size of <see cref="Controls.VideoPlayerView.TimestampFontSize"/>
    /// </summary>
    public double TimestampFontSize
    {
        get => _timeStampFontSize;
        set => SetValue(ref _timeStampFontSize, value);
    }
    private double _timeStampFontSize = 14d;
    
    public Thickness PlayerBorderMargin
    {
        get => _playerBorderMargin;
        set => SetValue(ref _playerBorderMargin, value);
    }
    private Thickness _playerBorderMargin = new Thickness(20,10);
    
    public double PlayPauseIconSize
    {
        get => _playPauseIconSize;
        set => SetValue(ref _playPauseIconSize, value);
    }
    private double _playPauseIconSize = 40d;
    
    public Color ViewBackgroundColor
    {
        get => _viewBackgroundColor;
        set => SetValue(ref _viewBackgroundColor, value);
    }
    private Color _viewBackgroundColor = Camera.MAUI.Ex.Resources.Colors.Snow;
    
    public Color PlayerBackgroundColor
    {
        get => _playerBackgroundColor;
        set => SetValue(ref _playerBackgroundColor, value);
    }
    private Color _playerBackgroundColor = Camera.MAUI.Ex.Resources.Colors.Snow;
    
    public Color TimestampBackgroundColor
    {
        get => _timestampBackgroundColor;
        set => SetValue(ref _timestampBackgroundColor, value);
    }
    private Color _timestampBackgroundColor = Color.FromArgb("#80000000");
    
    public Color TimestampTextColor
    {
        get => _timestampTextColor;
        set => SetValue(ref _timestampTextColor, value);
    }
    private Color _timestampTextColor = Camera.MAUI.Ex.Resources.Colors.Snow;
    
    public string TimestampFontFamily
    {
        get => _timestampFontFamily;
        set => SetValue(ref _timestampFontFamily, value);
    }
    private string _timestampFontFamily = FontNames.PromptRegular;
    
    public Thickness TimestampMargin
    {
        get => _timestampMargin;
        set => SetValue(ref _timestampMargin, value);
    }
    private Thickness _timestampMargin = new Thickness(16,0,0,16);
    
    public Thickness TimestampPadding
    {
        get => _timestampPadding;
        set => SetValue(ref _timestampPadding, value);
    }
    private Thickness _timestampPadding = new Thickness(4,3,4,3);
    
    public double ThumbnailHeight
    {
        get => _thumbnailHeight;
        set => SetValue(ref _thumbnailHeight, value);
    }
    private double _thumbnailHeight = 40d;
    
    public double PositionBarHeight
    {
        get => _positionBarHeight;
        set => SetValue(ref _positionBarHeight, value);
    }
    private double _positionBarHeight = 40d;
    
    public Color SliderPositionBarColor
    {
        get => _sliderPositionBarColor;
        set => SetValue(ref _sliderPositionBarColor, value);
    }
    private Color _sliderPositionBarColor = Camera.MAUI.Ex.Resources.Colors.Blackbird;
    
    public double SliderPositionBarStartingPoint
    {
        get => _sliderPositionBarStartingPoint;
        set => SetValue(ref _sliderPositionBarStartingPoint, value);
    }
    private double _sliderPositionBarStartingPoint = 0;
    
    public double SliderPositionBarWidth
    {
        get => _sliderPositionBarWidth;
        set => SetValue(ref _sliderPositionBarWidth, value);
    }
    private double _sliderPositionBarWidth = 3d;
    
    public Thickness SliderBaseMargin
    {
        get => _sliderBaseMargin;
        set => SetValue(ref _sliderBaseMargin, value);
    }
    private Thickness _sliderBaseMargin = new Thickness(16,8,16,8);
    
    private ObservableCollection<Thumbnail> Thumbnails { get; } = new ObservableCollection<Thumbnail>();
    public IReadOnlyCollection<Thumbnail> ThumbnailItems => Thumbnails;
    #endregion

    public void ClearThumbnails()
    {
        Thumbnails.Clear();
    }

    public void AddThumbnail(Thumbnail thumbnail)
    {
        Thumbnails.Add(thumbnail);
        
        OnPropertyChanged(nameof(ThumbnailItems));
    }

    public string GUID = Guid.NewGuid().ToString();
    
    public VideoService(IVideoFileService fileServ = null)
    {
        Debug.WriteLine($"===== {nameof(VideoService)} ctor: {GUID}");

        Core.Initialize();

        PlayCommand = new Command(PlayControlCommand);
        NextCommand = new AsyncRelayCommand(TrimVideo);
        BackCommand = new AsyncRelayCommand(HandleBackNavigation);
        SeekPlayer = new Command<double>(SeekPlayerCommandImpl);

        _fileServ = fileServ ?? new VideoFileService();

        //**todo: improvement - dispose of this when the service is disposed.
        _seekEvents.Subscribe(position =>
        {
            Debug.WriteLine($"===== SeekPlayerCommand (background): {position}");
            if (CurrentPlayer != null)
            {
                bool wasPlaying = CurrentPlayer.IsPlaying;
                // Perform the seek operation on the media player
                // improvement: may need a semaphore?
                if (CurrentPlayer == null) { return; }
                CurrentPlayer.Pause();
                CurrentPlayer.Position = (float)position;
                CurrentPlayer.Pause();
                // update the video preview, even if it is not playing when seek performed
                //CurrentPlayer.Play();

                // pause if not playing before seek
                //if (wasPlaying == false) { CurrentPlayer.Pause(); }
            }
        });
    }

    protected virtual void SeekPlayerCommandImpl(double args)
    {
        Debug.WriteLine($"===== {nameof(SeekPlayerCommandImpl)}: {args}");
        if (args is double position
            && 
            position >= 0)
        {
            _seekEvents.OnNext(position);
        }
    }

    ///<inheritdoc/>
    public VlcMediaPlayer CreateMediaPlayer(string videoPath, bool isVideoPreview, CameraOrientation orientation)
    {
        Debug.WriteLine("Video Service Orientation: " + orientation.ToString());
        
        if (CurrentPlayer != null)
        {
            CurrentPlayer.Stop();
        }

        LibVLC libVlc = BuildOptions(isVideoPreview, orientation);
        VlcMediaPlayer mediaPlayer = BuildPlayer(videoPath, libVlc);

        FilePath = videoPath;
        var oldPLayer = CurrentPlayer;
        // set this up before firing events for CurrentPlayer changed.
        SetupPositionObservable(mediaPlayer);
        CurrentPlayer = mediaPlayer;
        // android will continuously throw tons of errors if we don't clean this up after removing it from the visual tree,
        // as it no longer has a reference to the view to draw on.
        oldPLayer?.Dispose();
        
        return CurrentPlayer;
    }

    private void SetupPositionObservable(VlcMediaPlayer player)
    {
        var obs = Observable.FromEventPattern<MediaPlayerPositionChangedEventArgs>(
                                               h => player.PositionChanged += h,
                                               h => player.PositionChanged -= h);

        //stop listening to old player
        _positionSubscriber?.Dispose();
        // unsubscribe all current subscribers, to help clean up UI code.
        _positionEvents?.Dispose();
        _positionEvents = new(1, new EventLoopScheduler());
        OnPropertyChanged(nameof(PositionEvents));
        //set up event handler for new player, down-sampling the events
        _positionSubscriber = obs.Sample(TimeSpan.FromMilliseconds(250), Scheduler.Default)
                                .Subscribe(args =>
                                {
                                    if (args.EventArgs.Position >= 0.95)
                                    {
                                        CurrentPlayer?.Pause();
                                    }
                                    // publish here
                                    _positionEvents.OnNext(args.EventArgs.Position);
                                });
    }

    private VlcMediaPlayer BuildPlayer(string videoPath, LibVLC libVlc)
    {
        var media = new Media(libVlc, new Uri(videoPath));

        var mediaPlayer = new VlcMediaPlayer(libVlc)
        {
            Media = media,
        };

        var track = media?.Tracks.FirstOrDefault(x => x.TrackType == TrackType.Video);

        VideoSize = new Size(track?.Data.Video.Width ?? 0,
                            track?.Data.Video.Height ?? 0);
        return mediaPlayer;
    }

    private static LibVLC BuildOptions(bool isVideoPreview, CameraOrientation orientation)
    {
        var transformType = orientation switch
        {
            CameraOrientation.LandscapeRight => 270f,
            CameraOrientation.LandscapeLeft => 90f,
            CameraOrientation.UpsideDown => 180f,
            _ => 0
        };
        
        var options = new List<string>
        {
            "--intf", "dummy",                // Disable any user interface
            "--no-video-title-show",       // Disable the display of the video title
            "--no-stats",                  // Disable statistical information
            "--no-sub-autodetect-file",    // Prevent automatic subtitle detection
            "--no-snapshot-preview",       // Disable snapshot preview images
            "--no-osd",                    // Disable on-screen display (e.g., filepath preview)
            "--avcodec-fast",              // Enable fast decoding
            "--network-caching=300",       // Reduce network caching time (in milliseconds)
        };

        // iOS works fine without the trasform option, but Android needs it for proper orientation handling
        if (DeviceInfo.Current.Platform == DevicePlatform.Android && orientation != CameraOrientation.Portrait)
        {
            options.Add("--video-filter=transform");  // Enable video transformations like rotation
            options.Add($"--transform-type={transformType}"); // Transform type based on orientation
        }

        if (!isVideoPreview)
        {
            options.Add("--no-audio");  // No audio decoding in preview mode
        }
        
        var libVlc = new LibVLC(options.ToArray());
        
        return libVlc;
    }

    private void HandlePositionChanged(object? sender, MediaPlayerPositionChangedEventArgs e)
    {
        Debug.WriteLine($"===== Media Player Position Changed: {e.Position}");

        //**todo: move into the publisher?
        if (e.Position >= 0.95)
        {
            CurrentPlayer?.Pause();
        }
    }

    ///<inheritdoc/>
    public async IAsyncEnumerable<Thumbnail?> GenerateThumbnailChain(Size sliderSize)
    {
        Debug.WriteLine($"=== {GenerateThumbnailChain} called, size:{sliderSize.Width}, {sliderSize.Height}.");
        float height = (float)sliderSize.Height;
        float width = (float)sliderSize.Width;

        // improvement use a task here to offload work from UI thread.
        VlcMediaPlayer mediaPlayer = CurrentPlayer;// BuildPlayer(videoPath, BuildOptions(false, orientation));
        //Setting the volume to 0 when generating thumbnails
        mediaPlayer.Volume = 0;
        var media = mediaPlayer.Media;
        await media.Parse();

        if (media.Duration <= 0)
        {
            Debug.WriteLine("Media parsing failed or media has no duration.");
            yield return null;
        }

        Debug.WriteLine($"=== setting up player event listeners.");

        var eventHandler = Observable.FromEventPattern<EventArgs>(h => mediaPlayer.Playing += h,
                                                                  h => mediaPlayer.Playing -= h);
        var cancel = new CancellationTokenSource();
        var playerStarted = eventHandler.Take(1).ToTask(cancel.Token);

        Debug.WriteLine($"=== player.IsPlaying: {mediaPlayer.IsPlaying}.");

        if (mediaPlayer.IsPlaying)
        {
            Debug.WriteLine($"... stopping.");
            var eventHandlerStop = Observable.FromEventPattern<EventArgs>(h => mediaPlayer.Stopped += h,
                                                                          h => mediaPlayer.Stopped -= h); 
            var playerStopped = eventHandlerStop.Take(1).ToTask();
            mediaPlayer.Stop();
            await playerStopped; // Give some time for the player to prepare
        }

        mediaPlayer.Play();
        Debug.WriteLine($"=== waiting for player to start, playing: {mediaPlayer.IsPlaying}.");
        cancel.CancelAfter(TimeSpan.FromSeconds(1)); // Cancel if it takes too long to start
        await playerStarted; // Wait for the player to start
        Debug.WriteLine("===== Media is playing.");

        mediaPlayer.Pause();
        Debug.WriteLine("===== Media paused.");

        double aspectRatio = GetAspectRatio(media);
        double widthRemaining = width;
        Debug.WriteLine($"aspectratio: {aspectRatio}");
        Debug.WriteLine($"widthRemaining: {widthRemaining}");

        // this set was throwing when videoService.ThumbnailWidthChanged was not subscribed to, stopping thumb-generation
        // but the exception was on a task, so no stack trace was visible in the console.
        // note on this math:
        // set the width of each thumbnail from slider's height on screen, back-calculate the width of the thumbnail we need from this.
        // The calculation depends on orientation _of the video_ since video's HxW _always_ assumes landscape
        // (due to history of video for DVD, TV, Movie, blu-ray is never "portrait").  
        // aspectRatio = width / height (**always** assuming landscape orientation), so we can use it to calculate the thumbnail width.
        // "potrait" video orientation is a special case: treaded as "sideways landscape" H x W remains such that W always > H, 
        // so that aspectRatio is always > 1.0.
        ThumbnailWidth = VideoOrientation == CameraOrientation.Portrait
                        ? height / aspectRatio
                        : height * aspectRatio;

        Debug.WriteLine($"ThumbnailWidth: {ThumbnailWidth}");

        // compute how many thumbs we need to fill the target area given by sliderSize param
        var quantity = Math.Round(width / ThumbnailWidth);

        //**todo: we can check here if there's enough of a gap from the round to +1 thumb?

        var durationInMilliseconds = (uint?)mediaPlayer.Media?.Duration;
        var intervalMilliseconds = (uint)((durationInMilliseconds ?? 0) / quantity);
        Debug.WriteLine($"===== generating {quantity} thumbnails.");

        uint timeMs = 0;
        for (var numberLeft = quantity; numberLeft > 0; numberLeft--)
        {
            yield return await GenerateThumbnail(height, ThumbnailWidth, timeMs).ConfigureAwait(false);

            timeMs += intervalMilliseconds;
        }

        Debug.WriteLine("Thumbnail extraction complete!");

        mediaPlayer.Volume = 100; // Setting the volume to 100 for the playback video
        mediaPlayer.Position = 0.0f; // Setting position bar back to 0
    }

    public async Task<Thumbnail?> GenerateThumbnail(double height, double width, long timeMs)
    {
        int retries = 5;
        bool retry = false;
        var mediaPlayer = CurrentPlayer;

        var fileName = Path.GetFileNameWithoutExtension(FilePath);
        var thumbnailDirectory = Path.Combine(FileSystem.CacheDirectory, $"{fileName}_thumbnails");
        if (!Directory.Exists(thumbnailDirectory))
        {
            Directory.CreateDirectory(thumbnailDirectory);
        }

        // Wait for media time update            
        var thumbnailFilePath = Path.Combine(thumbnailDirectory, $"thumbnail_{timeMs}.png");

        do
        {
            mediaPlayer.Time = timeMs;
            mediaPlayer.Play();

            // This code fixes the issue on iOS where the beginning frames are significantly darker
            // than the remaining frames. The downside of this code is that it puts a floor on the time
            // of the first useable frame. TODO: It would be good to find another way to improve this as
            // this could affect thumbnail selection.
            if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
            {
                const int minIosDelay = 500;
                if (mediaPlayer.Time < minIosDelay)
                    await Task.Delay(minIosDelay - (int)mediaPlayer.Time);
            }

            //VlcSharp is being weird with height/width swapped (it appears on this method), which was why the thumbnails were 
            // stretched horizontally.  Seems to be true for both Android and iOS.
            // DO NOT TRUST VLCsharp's hints here, there is a bug
            bool shot = false;
            
            bool iosLandscape = DeviceInfo.Current.Platform == DevicePlatform.iOS
                                &&
                                (VideoOrientation == CameraOrientation.LandscapeLeft
                                 || 
                                 VideoOrientation == CameraOrientation.LandscapeRight);
            
            shot = iosLandscape 
                   ? mediaPlayer.TakeSnapshot(0, thumbnailFilePath, (uint)width, (uint)height) 
                   : mediaPlayer.TakeSnapshot(0, thumbnailFilePath, (uint)height, (uint)width);
            
            await Task.Delay(10).ConfigureAwait(false);
            mediaPlayer.Pause();

            if (shot)
            {
                if (File.Exists(thumbnailFilePath))
                {
                    RotateThumbnail(thumbnailFilePath, VideoOrientation);
                    OnThumbnailGenerated(thumbnailFilePath);
                    Debug.WriteLine($"Thumbnail saved at {thumbnailFilePath}");
                }
                else
                {
                    retry = true;
                    // allow some time before retrying... this only seems to happen on the 
                    // first snapshot after the video is played.
                    await Task.Delay(50).ConfigureAwait(false);
                }
            }
            else
            {
                retry = true;
            }

            if (retry)
            {
                Debug.WriteLine("Snapshot file not found after taking the snapshot.");
                // Cheesy retry. TODO: Make this better.
                if (retries > 0)
                {
                    retries--;
                }
            }

        } while (retry && retries > 0);

        return new Thumbnail()
        {
            ImageUrl = thumbnailFilePath,
            TimestampMs = timeMs
        };
    }

    private void OnThumbnailGenerated(string thumbnailFilePath) =>
                    MainThread.BeginInvokeOnMainThread(() => ThumbnailGenerated?.Invoke(this, thumbnailFilePath));

    private double GetAspectRatio(Media? media)
    {
        double aspectRatio = VideoSize!.Value.Width / VideoSize!.Value.Height;//16.0 / 9.0;
        var videoTrack = media?.Tracks.FirstOrDefault(x => x.TrackType == TrackType.Video);
        if (videoTrack != null)
        {
            var mediaWidth = videoTrack?.Data.Video.Width;
            var mediaHeight = videoTrack?.Data.Video.Height;

            if (mediaWidth > 0 && mediaHeight > 0)
                aspectRatio = (double)mediaWidth / (double)mediaHeight;
        }

        return aspectRatio;
    }

    private void PlayControlCommand()
    {
        Debug.WriteLine("===== PlayControlCommand called.");
        Debug.WriteLine($"===== CurrentPlayer.IsPlaying: {CurrentPlayer.IsPlaying}");
        Debug.WriteLine($"===== CurrentPlayer.Position: {CurrentPlayer.Position}");
        //** todo: print some guid of the player here?
        //** seems like we can't control it anymore?
        if (CurrentPlayer.IsPlaying)
        {
            CurrentPlayer.Pause();
        }
        else
        {
            if (CurrentPlayer.Position >= 0.95)
            {
                // if we are at the end of the video, reset to the beginning before playing again
                CurrentPlayer.Position = 0;
            }
            
            CurrentPlayer.Play();
        }
    }

    private async Task TrimVideo()
    {
        var res = await TrimVideo(FilePath, 0, 0);// TrimFrom, TrimTo);

        if (res != null)
        {
            Debug.WriteLine(res);
        }
        else
        {
            Debug.WriteLine("ERROR TRIMMING");
        }
    }

    protected virtual async Task HandleBackNavigation()
    {
        // this relies on to have the correct file path set in CreateVideoPlayer
        await _fileServ.DeleteVideoFile(FilePath);

        await Shell.Current.GoToAsync("..");
    }

    private static int GetHeight(Media? media)
    {
        var videoTrack = media?.Tracks.FirstOrDefault(x => x.TrackType == LibVLCSharp.Shared.TrackType.Video);
        var height = videoTrack?.Data.Video.Height;

        return (int)height;
    }
    
    private static void RotateThumbnail(string filePath, CameraOrientation? orientation)
    {
        using var inputStream = File.OpenRead(filePath);
        using var bitmap = SKBitmap.Decode(inputStream);

        var rotatedBitmap = new SKBitmap(bitmap.Height, bitmap.Width);
        using (var canvas = new SKCanvas(rotatedBitmap))
        {
            canvas.Clear(SKColors.Transparent);
            canvas.Translate(rotatedBitmap.Width / 2, rotatedBitmap.Height / 2);

            var rotationAngle = 0f;
            if (DeviceInfo.Current.Platform == DevicePlatform.Android)
            {
                //There's an issue with android mirroring the captured image, so we need to adjust the rotation angle accordingly.
                // https://github.com/janusw/CameraMaui/issues/21
                rotationAngle = 270f;
            }
            else
            {
                // iOS and iPad
                rotationAngle = orientation switch
                {
                    CameraOrientation.LandscapeLeft => 180f,
                    CameraOrientation.LandscapeRight => 0f,
                    CameraOrientation.UpsideDown => 270f,
                    CameraOrientation.Portrait => 90f,
                    _ => 0f
                };
            }

            canvas.RotateDegrees(rotationAngle);
            canvas.Translate(-bitmap.Width / 2, -bitmap.Height / 2);
            canvas.DrawBitmap(bitmap, 0, 0);
        }

        using var outputStream = File.OpenWrite(filePath);
        rotatedBitmap.Encode(outputStream, SKEncodedImageFormat.Png, 100);
    }

    //added this method here to prevent complex dependency injection issues
    /// <summary>
    /// Trims a video file to the specified start and end times and saves the trimmed video to a temporary location.
    /// </summary>
    /// <param name="videoPath">The path to the original video file to be trimmed.</param>
    /// <param name="startMs">The start time in milliseconds for trimming the video.</param>
    /// <param name="endMs">The end time in milliseconds for trimming the video.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the path to the trimmed video file if successful, or null if the trimming failed.</returns>
    public Task<string?> TrimVideo(string videoPath, double startMs, double endMs)
    {
        var fileName = Path.GetFileNameWithoutExtension(videoPath);
        var trimmedVideo = Path.Combine(FileSystem.CacheDirectory, $"{fileName}_trimmed.mp4");
        var taskCompletionSource = new TaskCompletionSource<string?>();
        var isCompleted = false;

        // Convert milliseconds to seconds
        var start = startMs / 1000.0;
        var end = endMs / 1000.0;

        Debug.WriteLine($"Starting trimming video {start:F3} to {end:F3}");

        //TODO: Needs improvement to get the exact time trimmed
        // libVLC option only accepts seconds or with decimal 0.5
        var options = new[]
        {
            $":start-time={start:F3}",
            $":stop-time={end:F3}",
            $":sout=#file{{dst={trimmedVideo}}}",
            ":no-sout-all",
            ":sout-keep",
            "--intf", "dummy",
            "--no-video-title-show",
            "--no-stats",
            "--no-sub-autodetect-file",
            "--no-snapshot-preview",
            "--no-osd",
            ":input-fast-seek=false",
            ":sout-mux-caching=2000",    // Increase mux caching
            "--no-drop-late-frames",      // Prevent late frames from being dropped
            ":avcodec-hw=none"            // Disable hardware acceleration
        };

        var libVlc = new LibVLC();
        var mediaPlayer = new LibVLCSharp.Shared.MediaPlayer(libVlc);
        var media = new Media(libVlc, new Uri(videoPath), options);
        mediaPlayer.Media = media;

        mediaPlayer.EndReached += async (sender, e) =>
        {
            if (isCompleted) return;
            isCompleted = true;

            await Task.Delay(500);
            mediaPlayer.Stop();

            var resultPath = File.Exists(trimmedVideo) ? trimmedVideo : null;
            Debug.WriteLine(resultPath != null ? $"Trimmed video saved at: {resultPath}" : "Trimming failed.");
            //**todo: needs improvement - return a better result here, like a Result<string?> that has a .Error property...
            taskCompletionSource.SetResult(resultPath);
        };

        mediaPlayer.EncounteredError += (sender, e) =>
        {
            if (isCompleted) return;
            isCompleted = true;
            Debug.WriteLine("Error encountered during playback.");
            taskCompletionSource.SetResult(null);
        };

        mediaPlayer.Play();

        return taskCompletionSource.Task;
    }

    private IVideoFileService _fileServ;
    // allows UI thread to offload the actual seek operation on the mediaplayer to a background thread
    // cache only the last seek event, so that we can drop some if seeking takes longer than UI updates
    private System.Reactive.Subjects.ReplaySubject<double> _seekEvents = new(1, new EventLoopScheduler());
    
    private System.Reactive.Subjects.ReplaySubject<float> _positionEvents = new(1, new EventLoopScheduler());

    private IDisposable _positionSubscriber;
}