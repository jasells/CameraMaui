using System;
using LibVLCSharp.Shared;
using Microsoft.Maui.Controls;

namespace Camera.MAUI.Ex.Controls;

//
// Summary:
//     Generic MAUI VideoView
public class VideoView : View
{
    //
    // Summary:
    //     Xamarin.Forms MediaPlayer databinded property
    public static readonly BindableProperty MediaPlayerProperty = BindableProperty.Create("MediaPlayer", typeof(LibVLCSharp.Shared.MediaPlayer), typeof(VideoView), null, BindingMode.OneWay, null, propertyChanging: OnMediaPlayerChanging, propertyChanged: OnMediaPlayerChanged);

    //
    // Summary:
    //     The MediaPlayer object attached to this view
    public LibVLCSharp.Shared.MediaPlayer? MediaPlayer
    {
        get
        {
            return GetValue(MediaPlayerProperty) as LibVLCSharp.Shared.MediaPlayer;
        }
        set
        {
            SetValue(MediaPlayerProperty, value);
        }
    }

    //
    // Summary:
    //     Raised when a new MediaPlayer is set and will be attached to the view
    public event EventHandler<MediaPlayerChangingEventArgs>? MediaPlayerChanging;

    //
    // Summary:
    //     Raised when a new MediaPlayer is set and attached to the view
    public event EventHandler<MediaPlayerChangedEventArgs>? MediaPlayerChanged;

    private static void OnMediaPlayerChanging(BindableObject bindable, object oldValue, object newValue)
    {
        VideoView videoView = (VideoView)bindable;
        videoView.MediaPlayerChanging?.Invoke(videoView, new MediaPlayerChangingEventArgs(oldValue as LibVLCSharp.Shared.MediaPlayer, newValue as LibVLCSharp.Shared.MediaPlayer));
    }

    private static void OnMediaPlayerChanged(BindableObject bindable, object oldValue, object newValue)
    {
        VideoView videoView = (VideoView)bindable;
        videoView.MediaPlayerChanged?.Invoke(videoView, new MediaPlayerChangedEventArgs(oldValue as LibVLCSharp.Shared.MediaPlayer, newValue as LibVLCSharp.Shared.MediaPlayer));
    }
}