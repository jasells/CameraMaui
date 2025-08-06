using Camera.MAUI.Ex.Controls;

namespace Camera.MAUI.Ex.Handlers;
using Microsoft.Maui.Handlers;
#if ANDROID
using VideoViewImpl = LibVLCSharp.Platforms.Android.VideoView;
#elif IOS
using VideoViewImpl = LibVLCSharp.Platforms.iOS.VideoView;
#elif WINDOWS
using VideoViewImpl = LibVLCSharp.Platforms.Windows.VideoView;
#else //.Net 8 or MacOS
using VideoViewImpl = LibVLCSharp.Shared.IVideoView;
#endif 

public class VideoViewHandler(IPropertyMapper mapper, CommandMapper commandMapper = null)
    : ViewHandler<VideoView, VideoViewImpl>(mapper, commandMapper)
{
    /// <summary>
    /// 
    /// </summary>
    public VideoViewHandler() : this(PropertyMapper)
    {
    }
    
    /// <summary>
    /// 
    /// </summary>
    public static IPropertyMapper<VideoView, VideoViewHandler> PropertyMapper = new PropertyMapper<VideoView, VideoViewHandler>(ViewMapper)
    {
        [nameof(VideoView.MediaPlayer)] = MapMediaPlayer
    };
    
    /// <inheritdoc />
    protected override void ConnectHandler(VideoViewImpl platformView) => base.ConnectHandler(platformView);

    /// <inheritdoc />
    protected override void DisconnectHandler(VideoViewImpl platformView) => base.DisconnectHandler(platformView);

    /// <inheritdoc />
    protected override VideoViewImpl CreatePlatformView()
    {
        string platform = ".Net 8";

#if ANDROID
        platform = "Android";
        return new VideoViewImpl(Context);
#elif IOS
        platform = "iOS";
        return new VideoViewImpl();
#elif WINDOWS
        platform = "WinUI";
        return new VideoViewImpl();

        //todo: add #elif for MacOS...

#else  //.net8 core has no impl, but we need to define it so that the package can be added to a .Net8 x-plat project
        // for linking purposes.  At runtime, the correct platform-specific lib will be loaded.
        throw new NotImplementedException($"This exception means the current target plaftorm: {platform} is not supported or correctly initialized. VLC needs platform-specific libs loaded.");
#endif
    }

    /// <summary>
    /// Attach mediaplayer to the native view
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="view"></param>
    public static void MapMediaPlayer(VideoViewHandler handler, VideoView view)
    {
        handler.PlatformView.MediaPlayer = view.MediaPlayer;
    }
}