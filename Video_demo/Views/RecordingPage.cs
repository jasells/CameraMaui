
using Camera.MAUI.Ex.Services;

namespace Video_Demo.Views;

/// <summary>
/// Example of C#-only implementation of a page hosting the <see cref="Camera.MAUI.Ex.Controls.VideoRecordView"/>.
/// </summary>
public class RecordingPage : Camera.MAUI.Ex.Controls.BaseVideoRecordPage
{
	public RecordingPage(ICameraService cameraService,
                           IVideoRecordingService recSrv,
						   IVideoService vidSrv)
	{
		//var recView = new Camera.MAUI.Ex.Controls.VideoRecordView();
  //      Content = recView;

		//InjectDependencies(recView, cameraService, recSrv, vidSrv);

		Content = IPlatformApplication.Current!.Services
						.GetRequiredService<Camera.MAUI.Ex.Controls.VideoRecordView>();	
    }
}