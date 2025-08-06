using System;
using System.Diagnostics;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;

namespace Camera.MAUI.Ex.Controls;

public partial class ImageHighlightSlider : Grid
{
    public static readonly BindableProperty FilePathProperty =
    BindableProperty.Create(
    nameof(FilePath),
    typeof(string),
    typeof(ImageHighlightSlider)
    );

    public static readonly BindableProperty MediaPlayerProperty =
    BindableProperty.Create(
    nameof(MediaPlayer),
    typeof(LibVLCSharp.Shared.MediaPlayer),
    typeof(ImageHighlightSlider)
    );


    // Position can be set by an attached video player during play, so it should not be wired to seek the video
    public static readonly BindableProperty PositionProperty =
    BindableProperty.Create(
    nameof(Position),
    typeof(double),
    typeof(ImageHighlightSlider)
    );
    
    public static readonly BindableProperty HighlightSliderMarginProperty =
        BindableProperty.Create(
            nameof(HighlightSliderMargin),
            typeof(Thickness),
            typeof(ImageHighlightSlider),
            new Thickness(30,0,30,0)
        );
    
    public static readonly BindableProperty SliderBaseMarginProperty =
        BindableProperty.Create(
            nameof(SliderBaseMargin),
            typeof(Thickness),
            typeof(ImageHighlightSlider),
            new Thickness(0,2,0,0)
        );
    
    public static readonly BindableProperty ImageSelectorHeightRequestProperty =
        BindableProperty.Create(
            nameof(ImageSelectorHeightRequest),
            typeof(double),
            typeof(ImageHighlightSlider),
            38d
        );
    
    public static readonly BindableProperty ImageSelectorWidthRequestProperty =
        BindableProperty.Create(
            nameof(ImageSelectorWidthRequest),
            typeof(double),
            typeof(ImageHighlightSlider),
            44d
        );
    
    public static readonly BindableProperty ImageSelectorCornerRadiusProperty =
        BindableProperty.Create(
            nameof(ImageSelectorCornerRadius),
            typeof(Thickness),
            typeof(ImageHighlightSlider),
            new Thickness(6,6,6,6)
        );

    public event EventHandler<double> SeekPlayer;

    /// <summary>
    /// Seek Command Parameter, for user controls to update an external player
    /// </summary>
    public static readonly BindableProperty SeekCommandParameterProperty =
        BindableProperty.Create(nameof(SeekCommandParameter),
        typeof(double),
        typeof(ImageSliderBase));

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
    
    public Thickness HighlightSliderMargin
    {
        get => (Thickness)GetValue(HighlightSliderMarginProperty);
        set => SetValue(HighlightSliderMarginProperty, value);
    }
    
    public Thickness SliderBaseMargin
    {
        get => (Thickness)GetValue(SliderBaseMarginProperty);
        set => SetValue(SliderBaseMarginProperty, value);
    }
    
    public Thickness ImageSelectorCornerRadius
    {
        get => (Thickness)GetValue(ImageSelectorCornerRadiusProperty);
        set => SetValue(ImageSelectorCornerRadiusProperty, value);
    }
    
    public double ImageSelectorHeightRequest
    {
        get => (double)GetValue(ImageSelectorHeightRequestProperty);
        set => SetValue(ImageSelectorHeightRequestProperty, value);
    }
    
    public double ImageSelectorWidthRequest
    {
        get => (double)GetValue(ImageSelectorWidthRequestProperty);
        set => SetValue(ImageSelectorWidthRequestProperty, value);
    }

    public LibVLCSharp.Shared.MediaPlayer MediaPlayer
    {
        get => (LibVLCSharp.Shared.MediaPlayer)GetValue(MediaPlayerProperty);
        set => SetValue(MediaPlayerProperty, value);
    }

    private double _leftBoxWidthRequest = 0;
    public double LeftBoxWidthRequest
    {
        get => _leftBoxWidthRequest;
        set
        {
            _leftBoxWidthRequest = value <= 0 ? 1 : value;;
            OnPropertyChanged();
        }
    }

    private double _rightBoxWidthRequest = 0;
    public double RightBoxWidthRequest
    {
        get => _rightBoxWidthRequest;
        set
        {
            _rightBoxWidthRequest = value <= 0 ? 1 : value;
            OnPropertyChanged();
        }
    }

    public ImageHighlightSlider()
    {
        InitializeComponent();

        this.SizeChanged += ImageHighlightSlider_SizeChanged;

        // This event is needed only for initialization, to keep the slider's Border within the bounds of the
        // highlighted images in the sliderBase
        Thumb.SizeChanged += ImageHighlightSlider_SizeChanged;
    }

    private void ImageHighlightSlider_SizeChanged(object? sender, EventArgs e)
    {


        if (sender == Thumb)
        {
            // This is needed because the thumb.SizedChanged event will causeflicker on the tap gesture.
            Thumb.SizeChanged -= ImageHighlightSlider_SizeChanged;
        }

        if (DeviceInfo.Current.Platform == DevicePlatform.iOS)
        {
            // Without setting this LeftBoxWidthRequest here, the opacity property does not display on iOS. The hack here is to set it
            // to a minimal non-zero number.
            LeftBoxWidthRequest = 1;
        }
        else
        {
            LeftBoxWidthRequest = 0;
        }

        RightBoxWidthRequest = Xmax - LeftBoxWidthRequest;
    }

    private double startX = 0;
    double Xmin => 0;
    double Xmax => Width - Thumb.Width;

    private void PanGestureRecognizer_PanUpdated(object sender, PanUpdatedEventArgs e)
    {
        var container = sender as ContentView;

        switch (e.StatusType)
        {
            case GestureStatus.Started:
                startX = container.X + container.TranslationX;
                Debug.WriteLine("-------- Pan Started: startX = {0}, Xmin = {1}, Xmax = {2} --------", startX, Xmin, Xmax);
                break;

            case GestureStatus.Running:
                // Translate the BoxView based on the pan gesture
                LeftBoxWidthRequest = Math.Clamp(startX + e.TotalX, Xmin, Xmax);
                RightBoxWidthRequest = Xmax - LeftBoxWidthRequest;
                SeekCommandParameter = LeftBoxWidthRequest / Xmax;
                SeekPlayer(this, SeekCommandParameter);
                Debug.WriteLine("-------- Pan Running: e.TotalX = {0}, Translation = {1} --------", e.TotalX, container.TranslationX);
                break;

            case GestureStatus.Completed:
                // Optionally, handle the completion of the gesture
                Debug.WriteLine("-------- Pan Completed: container.TranslationX = {0} --------", container.TranslationX);
                break;
        }
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        var point = e.GetPosition(SliderBase);
        var startX = point.Value.X - Thumb.Width / 2;
        LeftBoxWidthRequest = Math.Clamp(startX, Xmin, Xmax);
        RightBoxWidthRequest = Xmax - LeftBoxWidthRequest;
        SeekCommandParameter = LeftBoxWidthRequest / Xmax;
        SeekPlayer(this, SeekCommandParameter);
        Debug.WriteLine("-------- Tapped: startX = {0} --------", startX);
    }
}