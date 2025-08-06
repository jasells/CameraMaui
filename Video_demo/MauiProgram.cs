#if DEBUG
using Microsoft.Extensions.Logging;
#endif

using Camera.MAUI;
using CommunityToolkit.Maui;

using Camera.MAUI.Ex;
using Camera.MAUI.Ex.Services;

using Video_Demo.ViewModel;
using Video_Demo.Views;
using Camera.MAUI.Ex.Controls;

namespace Video_Demo;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            // adds CameraService, VideoService, and VideoRecordingService
            .UseMauiVideo()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Pages
        builder.Services.AddTransient<VideoRecordPage>();
        builder.Services.AddTransient<SettingsPage>();
        builder.Services.AddTransient<ThumbnailSelectionPage>();
        builder.Services.AddTransient<TrimmingPage>();
        builder.Services.AddTransient<VideoPreviewPage>();

        // ViewModels
        builder.Services.AddTransient<VideoRecordViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();
        builder.Services.AddTransient<ThumbnailSelectionViewModel>();
        builder.Services.AddTransient<TrimmingViewModel>();
        builder.Services.AddTransient<VideoPreviewViewModel>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}

