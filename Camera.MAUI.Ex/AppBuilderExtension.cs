using Camera.MAUI.Ex.Controls;
using Camera.MAUI.Ex.Handlers;
using Camera.MAUI.Ex.Resources;
using Camera.MAUI.Ex.Services;
using Camera.MAUI;
using CommunityToolkit.Maui;
using Debug = System.Diagnostics.Debug;


namespace Camera.MAUI.Ex;

public static class AppBuilderExtensions
{

    /// <summary>
    /// Supports Android, iOS, and Windows. No Mac support, currently.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="registerServices"> set to <c>false</c> to use auto-fac or some other DI system, and only init dependency packages, like <see cref="Camera.MAUI.AppBuilderExtensions.UseMauiCameraView(MauiAppBuilder)"/>.</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static MauiAppBuilder UseMauiVideo(this MauiAppBuilder builder, 
                                             bool registerServices = true)
    {
        builder.UseMauiCommunityToolkit()
            .UseMauiCameraView();
        builder.ConfigureMauiHandlers(handlers =>
        {
            string platform = ".Net 8";

#if ANDROID
            platform = "Android";
#elif IOS
            platform = "iOS";
#elif WINDOWS
            platform = "WinUI";
#endif
            //todo: add #elif for MacOS...

#if ANDROID || IOS || WINDOWS
            Debug.WriteLine($"adding VideoViewHandler for current platform: {platform}.");
            handlers.AddHandler(typeof(VideoView), typeof(VideoViewHandler));

#else   //.net8 core has no impl, but we need to define it so that the package can be added to a .Net8 x-plat project
        // for linking purposes.  At runtime, the correct platform-specific lib will be loaded.
            throw new NotImplementedException($"This exception means the current target platform: {platform} is not supported or correctly initialized. VLC needs platform-specific libs loaded, so make sure the correct platform-specific packages and targets are configured in your application project.");
#endif
        });
        
        builder.ConfigureFonts(fonts =>
        {
            fonts.AddFont("PromptRegular.ttf", FontNames.PromptRegular);
        });

        // Services
        if (registerServices)
        {
            builder.Services.AddSingleton<ICameraService, CameraService>();
            builder.Services.AddSingleton<IVideoService, VideoService>();
            builder.Services.AddSingleton<IVideoRecordingService, VideoRecordingService>();
            builder.Services.AddSingleton<IVideoFileService, VideoFileService>();
            builder.Services.AddSingleton<IVideoRequirementsService, VideoRequirementsService>();
            builder.Services.AddSingleton<IUploadProgressService, UploadProgressService>();

            // add views here?
            builder.Services.AddTransient<VideoPreview>();
            builder.Services.AddTransient<VideoRecordView>();
            builder.Services.AddTransient<UploadProgress>();
        }

        return builder;
    }
}