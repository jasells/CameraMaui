using Microsoft.Maui.Controls.Shapes;

namespace Camera.MAUI.Ex.Controls;

public partial class PlayPauseButton
{
    public static readonly BindableProperty IconSizeProperty = BindableProperty.Create(
        nameof(IconSize),
        typeof(double),
        typeof(PlayPauseButton),
        40d,
        propertyChanged: OnIconSizeChanged);
    
    public static readonly BindableProperty IsPlayingProperty = BindableProperty.Create(
        nameof(IsPlaying),
        typeof(bool),
        typeof(PlayPauseButton),
        false);
    
    public double IconSize
    {
        get => (double)GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }
    
    public bool IsPlaying
    {
        get => (bool)GetValue(IsPlayingProperty);
        set => SetValue(IsPlayingProperty, value);
    }
    
    public PlayPauseButton()
    {
        InitializeComponent();
    }
    
    private static void OnIconSizeChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is PlayPauseButton button
            && newValue is double size)
        {
            button.This.HeightRequest = size;
            button.This.WidthRequest = size;

            var half = size / 2d;
            
            button.This.StrokeShape = new RoundRectangle
            {
                CornerRadius = half
            };
            
            button.Icon.HeightRequest = half;
            button.Icon.WidthRequest = half;
        }
    }
}