using Camera.MAUI.Ex.Services;

namespace Camera.MAUI.Ex.Controls;

public abstract class BaseVideoRecordPage : ContentPage, IVideoViewParent
{
	public BaseVideoRecordPage()
	{

    }

    public event Func<Task> NavigatedToAsync;

	protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        if (NavigatedToAsync != null)
        {
            await NavigatedToAsync.Invoke();
        }

        base.OnNavigatedTo(args);
    }

    /// <summary>
    /// Injects the dependencies into the child view.  Allows for app-level IoC flexibility to use 
    /// any IoC framework desired, not just the MS DependencyInjection framework.
    /// </summary>
    /// <param name="childView"></param>
    /// <param name="cameraService"></param>
    /// <param name="recSrv"></param>
    protected void InjectDependencies(VideoRecordView childView,
                                        ICameraService cameraService,
                                        IVideoRecordingService recSrv,
                                        IVideoService vidSrv)
    {
        childView.InjectDependencies(cameraService, recSrv, vidSrv);
    }
}