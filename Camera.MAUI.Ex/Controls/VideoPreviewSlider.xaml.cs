using Camera.MAUI.Ex.Services;
using System.ComponentModel;
using System.Diagnostics;
using System.Reactive.Linq;

namespace Camera.MAUI.Ex.Controls;

public partial class VideoPreviewSlider : INavAwareControl
{
    #region bindable props
    
    public event EventHandler<double> SeekPlayer;

    /// <summary>
    /// Seek Command Parameter, for user controls to update an external player
    /// </summary>
    public static readonly BindableProperty SeekCommandParameterProperty =
        BindableProperty.Create(nameof(SeekCommandParameter),
        typeof(double),
        typeof(VideoPreviewSlider));

    public double SeekCommandParameter
    {
        get { return (double)GetValue(SeekCommandParameterProperty); }
        set { SetValue(SeekCommandParameterProperty, value); }
    }
    #endregion  

    public IVideoService VideoService
    {
        get => _videoService;
        private set
        {
            _videoService = value;
            OnPropertyChanged();
        }
    }
    private IVideoService _videoService;
    
    public VideoPreviewSlider()
    {
        InitializeComponent();

        //**improvement: this should give smoother bar-movement during video playback...?
        // observation showed that mediaPLayer.Position only updates when the event is fired, ~4Hz,
        // even polling doesn't make the position movement any smoother.
        // We need some sort of animation to smooth out the movement between updates from player.
        //_positionTimer = new System.Timers.Timer(20);
        //_positionTimer.Elapsed += _positionTimer_Elapsed;
        //_positionTimer.Enabled = true;
    }

    public void InjectDependencies(IVideoService vidSrv)
    {
        VideoService = vidSrv;
        
        InternalSlider.InjectDependencies(VideoService);
    }

    public void OnNavigatedTo(object sender, EventArgs e)
    {
        Debug.WriteLine($"==== {nameof(VideoPreviewSlider)}.OnNavigatedTo");


        var obs = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>
                                (h => VideoService.PropertyChanged += h,
                                 h => VideoService.PropertyChanged -= h)
                             .Select(e => e.EventArgs)
                             .Where(args => args.PropertyName == nameof(VideoService.CurrentPlayer));
        SetupPlayerChangedObserver(obs);
        SetupPositionChangedEvent();

        InternalSlider.OnNavigatedTo(sender, e);
    }

    public void OnNavigatedFrom(object sender, EventArgs e)
    {
        Debug.WriteLine($"==== {nameof(VideoPreviewSlider)}.OnNavigatedFrom");
        //un-sub from the player changed events
        _playerChangedObserver?.Dispose();
        //un-sub from the position changed events
        _positionChangedObserver?.Dispose();
        InternalSlider.OnNavigatedFrom(sender, e);
    }

    private void SetupPlayerChangedObserver(IObservable<PropertyChangedEventArgs> obs)
    {
        //un-sub from propertyChanged events when off-screen to allow this control to be garbage collected
        _playerChangedObserver?.Dispose();
        _playerChangedObserver =
        obs.Subscribe(args =>
        {
            Debug.WriteLine($"VideoPreviewSlider: MediaPlayer changed: {args.PropertyName}");
            SetupPositionChangedEvent();
        });
    }

    private void SetupPositionChangedEvent()
    {
        //un-sub from the old player PositionChanged events
        _positionChangedObserver?.Dispose();
        // observe events from new player
        _positionChangedObserver =
            VideoService.PositionEvents.Subscribe(position =>
        {
            Debug.WriteLine($"==== {nameof(VideoPreviewSlider)}.PositionEvents - Position: {position}");
            SetSliderPosition(position);
        });
    }
    
    protected void SetSliderPosition(float position)
    {
        if (_panning == false)
        {
            var availableWidth = SlideGrid.Width + VideoService.SliderBaseMargin.Right;
            var positionAdj = Math.Min(position, 1.0);
            PositionBarBubble.TranslationX = (positionAdj * availableWidth);
        }
    }

    private void PositionBar_PanUpdated(object sender, PanUpdatedEventArgs e)
    {
        if (sender is ContentView container && container == PositionBarBubble)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    _startX = container.TranslationX;
                    _panning = true;
                    Debug.WriteLine("-------- Pan Started: startX = {0}, Xmin = {1}, Xmax = {2} --------", _startX, 0, SlideGrid.Width);
                    break;

                case GestureStatus.Running:
                    // Translate the BoxView based on the pan gesture
                    var availableWidth = SlideGrid.Width + VideoService.SliderBaseMargin.Right;
                    container.TranslationX = Math.Clamp(_startX + e.TotalX, 0, availableWidth);
                    SeekCommandParameter = container.TranslationX / availableWidth;
                    // must always null-check an event before invoking!
                    RaiseSeekPlayer();
                    Debug.WriteLine("-------- Pan Running: e.TotalX = {0}, Translation = {1} --------", e.TotalX, container.TranslationX);
                    break;

                case GestureStatus.Completed:
                    // Optionally, handle the completion of the gesture
                    _panning = false;
                    Debug.WriteLine("-------- Pan Completed: container.TranslationX = {0} --------", container.TranslationX);
                    break;
            }
        }
    }
    
    private void RaiseSeekPlayer() => SeekPlayer?.Invoke(this, SeekCommandParameter);

    private void TapGestureRecognizer_Tapped(object? sender, TappedEventArgs e)
    {
        var point = e.GetPosition(SlideGrid);
        if (point.HasValue)
        {
            var availableWidth = SlideGrid.Width - PositionBarBubble.Width;
            PositionBarBubble.TranslationX = Math.Clamp(point.Value.X, 0, availableWidth);
            SeekCommandParameter = point.Value.X / availableWidth;
            RaiseSeekPlayer();
            Debug.WriteLine("-------- Tapped: startX = {0}, translation = {1} --------",
                point.Value.X, PositionBarBubble.TranslationX);
        }
    }

    private volatile bool _panning = false;
    private IDisposable _playerChangedObserver;
    private IDisposable _positionChangedObserver;
    private double _startX;
}