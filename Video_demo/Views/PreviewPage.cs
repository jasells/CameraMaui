using Camera.MAUI.Ex.Controls;
using Camera.MAUI.Ex.Services;

namespace Video_Demo.Views;

/// <summary>
/// Example of C#-only implementation of a page hosting the <see cref="Camera.MAUI.Ex.Controls.VideoPreview"/>.
/// </summary>
public class PreviewPage : ContentPage
{
    /// <summary>
    /// This method _requires_ calling <see cref="Camera.MAUI.Ex.AppBuilderExtensions.UseMauiVideo(MauiAppBuilder, bool)"/>
    /// </summary>
    public PreviewPage()
    {
        // allow MS-Dependency injection to provide the dependencies/services
        var preview = IPlatformApplication.Current?.Services.GetService<VideoPreview>();

        Content = preview;
    }

    /// <summary>
    /// Inject services manually, allowing for other IoC frameworks to be used, or for testing purposes.
    /// </summary>
    /// <param name="cameraService"></param>
    /// <param name="videoSvc"></param>
    public PreviewPage(ICameraService cameraService,
                        IVideoService videoSvc)
    {
        Content = new VideoPreview(cameraService, videoSvc);
    }
}