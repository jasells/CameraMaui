using System;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Colors = Camera.MAUI.Ex.Resources.Colors;

namespace Camera.MAUI.Ex.Controls;

public partial class VideoTrimmingSlider
{
    public static readonly BindableProperty FilePathProperty =
        BindableProperty.Create(
        nameof(FilePath),
        typeof(string),
        typeof(ImageSliderBase)
        );

    public static readonly BindableProperty MediaPlayerProperty =
        BindableProperty.Create(
        nameof(MediaPlayer),
        typeof(LibVLCSharp.Shared.MediaPlayer),
        typeof(ImageSliderBase)
        );

    // Position can be set by an attached video player during play, so it should not be wired to seek the video
    public static readonly BindableProperty PositionProperty =
        BindableProperty.Create(
        nameof(Position),
        typeof(double),
        typeof(ImageSliderBase)
        );
    
    public static readonly BindableProperty TrimFromProperty =
        BindableProperty.Create(
            nameof(TrimFrom),
            typeof(double),
            typeof(ImageSliderBase)
        );
    
    public static readonly BindableProperty TrimToProperty =
        BindableProperty.Create(
            nameof(TrimTo),
            typeof(double),
            typeof(ImageSliderBase)
        );
    
    public static readonly BindableProperty SliderBaseMarginProperty =
        BindableProperty.Create(
            nameof(SliderBaseMargin),
            typeof(Thickness),
            typeof(ImageSliderBase),
            new Thickness(16,8,16,8)
        );
    
    public static readonly BindableProperty ThumbWidthProperty =
        BindableProperty.Create(
            nameof(ThumbWidth),
            typeof(double),
            typeof(ImageSliderBase),
            18d
        );
    
    public static readonly BindableProperty ThumbMarginProperty =
        BindableProperty.Create(
            nameof(ThumbMargin),
            typeof(Thickness),
            typeof(ImageSliderBase),
            new Thickness(0,8,0,8)
        );
    
    public static readonly BindableProperty SliderHeightProperty =
        BindableProperty.Create(
            nameof(SliderHeight), //top bot
            typeof(double),
            typeof(ImageSliderBase),
            2d
        );
    
    public static readonly BindableProperty SliderColorProperty =
        BindableProperty.Create(
            nameof(SliderColor), //top bot
            typeof(Color),
            typeof(ImageSliderBase),
            Colors.Leaf
        );
    
    public static readonly BindableProperty PositionBarStartingPointProperty =
        BindableProperty.Create(
            nameof(PositionBarStartingPoint),
            typeof(double),
            typeof(ImageSliderBase),
            20d
        );
    
    public static readonly BindableProperty PositionBarWidthProperty =
        BindableProperty.Create(
            nameof(PositionBarWidth), //top bot
            typeof(double),
            typeof(ImageSliderBase),
            2d
        );
    
    public static readonly BindableProperty PositionBarColorProperty =
        BindableProperty.Create(
            nameof(PositionBarColor), //top bot
            typeof(Color),
            typeof(ImageSliderBase),
            Colors.Blackbird
        );

    public event EventHandler<double> SeekPlayer;

    /// <summary>
    /// Seek Command Parameter, for user controls to update an external player
    /// </summary>
    public static readonly BindableProperty SeekCommandParameterProperty =
        BindableProperty.Create(nameof(SeekCommandParameter),
        typeof(double),
        typeof(VideoTrimmingSlider));

    public double SeekCommandParameter
    {
        get { return (double)GetValue(SeekCommandParameterProperty); }
        set { SetValue(SeekCommandParameterProperty, value); }
    }

    public double Position
    {
        get => (double)GetValue(PositionProperty);
        set => SetValue(PositionProperty, value);
    }

    public string FilePath
    {
        get => (string)GetValue(FilePathProperty);
        set => SetValue(FilePathProperty, value);
    }
    
    public double TrimFrom
    {
        get => (double)GetValue(TrimFromProperty);
        set
        {
            SetValue(TrimFromProperty, value);
            OnPropertyChanged();
        }
    }
    
    public double TrimTo
    {
        get => (double)GetValue(TrimToProperty);
        set
        {
            SetValue(TrimToProperty, value);
            OnPropertyChanged();
        }
    }
    
    public Thickness SliderBaseMargin
    {
        get => (Thickness)GetValue(SliderBaseMarginProperty);
        set => SetValue(SliderBaseMarginProperty, value);
    }
    
    public double ThumbWidth
    {
        get => (double)GetValue(ThumbWidthProperty);
        set => SetValue(ThumbWidthProperty, value);
    }
    
    public Thickness ThumbMargin
    {
        get => (Thickness)GetValue(ThumbMarginProperty);
        set => SetValue(ThumbMarginProperty, value);
    }
    
    public double SliderHeight
    {
        get => (double)GetValue(SliderHeightProperty);
        set => SetValue(SliderHeightProperty, value);
    }
    
    public Color SliderColor
    {
        get => (Color)GetValue(SliderColorProperty);
        set => SetValue(SliderColorProperty, value);
    }
    
    public double PositionBarStartingPoint
    {
        get => (double)GetValue(PositionBarStartingPointProperty);
        set => SetValue(PositionBarStartingPointProperty, value);
    }
    
    public double PositionBarWidth
    {
        get => (double)GetValue(PositionBarWidthProperty);
        set => SetValue(PositionBarWidthProperty, value);
    }
    
    public Color PositionBarColor
    {
        get => (Color)GetValue(PositionBarColorProperty);
        set => SetValue(PositionBarColorProperty, value);
    }

    public LibVLCSharp.Shared.MediaPlayer MediaPlayer
    {
        get => (LibVLCSharp.Shared.MediaPlayer)GetValue(MediaPlayerProperty);
        set => SetValue(MediaPlayerProperty, value);
        // TODO? : We could subscribe to events (MediaPlayer.Playing and MediaPlayer.Stopped) here to enabled/disable
        // the timer for position bar updates.
    }

    private double _leftBoxWidthRequest = 0;
    public double LeftBoxWidthRequest
    {
        get => _leftBoxWidthRequest;
        set
        {
            _leftBoxWidthRequest = value;
            OnPropertyChanged();
        }
    }

    private double _rightBoxWidthRequest = 0;
    public double RightBoxWidthRequest
    {
        get => _rightBoxWidthRequest;
        set
        {
            _rightBoxWidthRequest = value;
            OnPropertyChanged();
        }
    }

    double positionBarLeftOffset;
    double positionBarRightOffset;
    private System.Timers.Timer _positionTimer;

    public VideoTrimmingSlider()
    {
        InitializeComponent();

        this.SizeChanged += VideoTrimmingSlider_SizeChanged;
        LeftThumb.SizeChanged += VideoTrimmingSlider_SizeChanged;
        RightThumb.SizeChanged += VideoTrimmingSlider_SizeChanged;
        PositionBarBubble.SizeChanged += PositionBar_SizeChanged;
        PositionBarBar.SizeChanged += PositionBar_SizeChanged;

        LeftThumb.XChanged += LeftThumb_XChanged;
        RightThumb.XChanged += RightThumb_XChanged;

        _positionTimer = new System.Timers.Timer(20);
        _positionTimer.Elapsed += _positionTimer_Elapsed;
        _positionTimer.Enabled = true;
    }

    private void _positionTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        // Another feature to enhance this would be to turn off the elapsed event when IsPlaying is changed. That's
        // probably ideal. Then, we're not firing so many events for no reason.
        if (MediaPlayer != null && MediaPlayer.IsPlaying)
        {
            //**todo: should look at MediaPlayer.PositionChanged or Forward event ToggledEventArgs update this
            //TODO: Need to add !Panning to the above condition
            PositionBarBubble.TranslationX = MediaPlayer.Position * RightThumb.Xmax + LeftThumb.Width;
        }
    }

    private void PositionBar_SizeChanged(object? sender, EventArgs e)
    {
        positionBarLeftOffset = -(PositionBarBubble.Width + PositionBarBar.Width) / 2;
        positionBarRightOffset = -(PositionBarBubble.Width - PositionBarBar.Width) / 2;

        PositionBarBubble.TranslationX = Math.Clamp(LeftThumb.Xval, LeftThumb.Xval + LeftThumb.Width + positionBarLeftOffset, RightThumb.Xval + positionBarLeftOffset);
    }

    private void RightThumb_XChanged(object? sender, double e)
    {
        var width = Width - RightThumb.Xval - RightThumb.Width;

        RightBoxWidthRequest = width < 0 ? 1 : width;
        
        TrimTo = (RightThumb.Xval / RightThumb.Xmax) * MediaPlayer.Length;
    }

    private void LeftThumb_XChanged(object? sender, double e)
    {
        LeftBoxWidthRequest = LeftThumb.Xval;
        
        TrimFrom = ((LeftThumb.Xval + LeftThumb.Width) / RightThumb.Xmax) * MediaPlayer.Length;
    }

    private bool sizeInitialized = false;

    private void VideoTrimmingSlider_SizeChanged(object? sender, EventArgs e)
    {
        LeftThumb.Xmin = 0;
        RightThumb.Xmax = Width - RightThumb.Width;

        if (!sizeInitialized && LeftThumb.Width > 0 && RightThumb.Width > 0)
        {
            LeftThumb.Xmax = RightThumb.Xmin - LeftThumb.Width;
            RightThumb.Xmin = LeftThumb.Xmax + LeftThumb.Width;

            LeftThumb.Xval = LeftThumb.Xmin;
            RightThumb.Xval = RightThumb.Xmax;

            barOffset = (PositionBarBubble.Width - PositionBarBar.Width) / 2;

            PositionBarBubble.TranslationX = Math.Clamp(startX, LeftThumb.Xval + LeftThumb.Width + positionBarLeftOffset, RightThumb.Xval + positionBarLeftOffset);
            sizeInitialized = true;
        }
    }

    double startX;
    double barOffset;

    private void Bookend_PanUpdated(object sender, PanUpdatedEventArgs e)
    {
        if (sender is TrimThumb thumb)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    if (thumb == LeftThumb)
                    {
                        startX = thumb.Xval;
                        thumb.Xmax = RightThumb.Xval - LeftThumb.Width;
                    }

                    if (thumb == RightThumb)
                    {
                        startX = thumb.Xval;
                        thumb.Xmin = LeftThumb.Xval + LeftThumb.Width;
                    }

                    Debug.WriteLine("-------- Pan Started: startX = {0:F2}, Left: Xval = {1:F2}, Xmin = {2:F2}, Xmax = {3:F2} - Right: Xval = {4:F2}, Xmin = {5:F2}, Xmax = {6:F2} --------",
                        startX, LeftThumb.Xval, LeftThumb.Xmin, LeftThumb.Xmax, RightThumb.Xval, RightThumb.Xmin, RightThumb.Xmax);
                    break;

                case GestureStatus.Running:
                    // Translate the BoxView based on the pan gesture
                    if (thumb == LeftThumb)
                    {
                        thumb.Xval = Math.Clamp(startX + e.TotalX, LeftThumb.Xmin, RightThumb.Xval - LeftThumb.Width);
                    }

                    else if (thumb == RightThumb)
                    {
                        thumb.Xval = Math.Clamp(startX + e.TotalX, LeftThumb.Xval + LeftThumb.Width, RightThumb.Xmax);
                    }

                    //SeekCommandParameter = LeftBoxWidthRequest / Xmax;
                    //SeekPlayer(this, SeekCommandParameter);
                    UpdatePositionWithBoundaries();
                    Debug.WriteLine("-------- Pan Running: e.TotalX = {0:F2}, Left: Xval = {1:F2}, Xmin = {2:F2}, Xmax = {3:F2} - Right: Xval = {4:F2}, Xmin = {5:F2}, Xmax = {6:F2} --------",
                        e.TotalX, LeftThumb.Xval, LeftThumb.Xmin, LeftThumb.Xmax, RightThumb.Xval, RightThumb.Xmin, RightThumb.Xmax);
                    break;

                case GestureStatus.Completed:
                    // Optionally, handle the completion of the gesture
                    Debug.WriteLine("-------- Pan Completed: container.TranslationX = {0} --------", thumb.TranslationX);
                    break;
            }
        }
    }

    private void UpdatePositionWithBoundaries()
    {
        var leftMin = LeftThumb.Xval + LeftThumb.Width + positionBarLeftOffset;
        var rightMax = RightThumb.Xval + positionBarRightOffset;

        bool seek = false;
        if (PositionBarBubble.TranslationX < leftMin)
        {
            PositionBarBubble.TranslationX = leftMin;
            seek = true;
        }
        else if (PositionBarBubble.TranslationX > rightMax)
        {
            PositionBarBubble.TranslationX = rightMax;
            seek = true;
        }

        if (seek)
        {
            SeekCommandParameter = PositionBarBubble.TranslationX / RightThumb.Xmax;
            RaiseSeekPlayer();
        }
    }

    private void PositionBar_PanUpdated(object sender, PanUpdatedEventArgs e)
    {
        if (sender is ContentView container && container == PositionBarBubble)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    startX = container.TranslationX;
                    Debug.WriteLine("-------- Pan Started: startX = {0}, Xmin = {1}, Xmax = {2} --------", startX, LeftThumb.Xmin, RightThumb.Xmax);
                    break;

                case GestureStatus.Running:
                    // Translate the BoxView based on the pan gesture
                    container.TranslationX = Math.Clamp(startX + e.TotalX, LeftThumb.Xval + LeftThumb.Width + positionBarLeftOffset, RightThumb.Xval + positionBarRightOffset);
                    SeekCommandParameter = container.TranslationX / RightThumb.Xmax;
                    // must always null-check an event before invoking!
                    RaiseSeekPlayer();
                    Debug.WriteLine("-------- Pan Running: e.TotalX = {0}, Translation = {1} --------", e.TotalX, container.TranslationX);
                    break;

                case GestureStatus.Completed:
                    // Optionally, handle the completion of the gesture
                    Debug.WriteLine("-------- Pan Completed: container.TranslationX = {0} --------", container.TranslationX);
                    break;
            }
        }
    }

    private void RaiseSeekPlayer() => SeekPlayer?.Invoke(this, SeekCommandParameter);

    private void TapGestureRecognizer_Tapped(object? sender, TappedEventArgs e)
    {
        var point = e.GetPosition(SlideGrid);


        var startX = point.Value.X;

        PositionBarBubble.TranslationX = Math.Clamp(startX, LeftThumb.Xval + LeftThumb.Width + positionBarLeftOffset, RightThumb.Xval + positionBarRightOffset);
        SeekCommandParameter = startX / RightThumb.Xmax;
        RaiseSeekPlayer();
        Debug.WriteLine("-------- Tapped: startX = {0}, translation = {1} , Left: Xval = {2}, BarOffset = {3}, Right: Xval = {4}, BarOffset = {5} --------",
            startX, PositionBarBubble.TranslationX, LeftThumb.Xval, positionBarLeftOffset, RightThumb.Xval, positionBarRightOffset);
    }

}