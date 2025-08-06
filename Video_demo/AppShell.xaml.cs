using Video_Demo.Views;

namespace Video_Demo;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		
		Routing.RegisterRoute(nameof(VideoRecordPage), typeof(VideoRecordPage));
		Routing.RegisterRoute(nameof(TrimmingPage), typeof(TrimmingPage));
		Routing.RegisterRoute(nameof(PreviewPage), typeof(PreviewPage));
		Routing.RegisterRoute(nameof(VideoPreviewPage), typeof(VideoPreviewPage));
        Routing.RegisterRoute(nameof(ThumbnailSelectionPage), typeof(ThumbnailSelectionPage));
        Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
        Routing.RegisterRoute(nameof(UploadTestPage), typeof(UploadTestPage));
    }
}

