using Camera.MAUI.Ex.Models;
using Video_Demo.ViewModel;

namespace Video_Demo.Views;

public partial class ThumbnailSelectionPage : ContentPage
{
    public ThumbnailSelectionViewModel? ViewModel => BindingContext as ThumbnailSelectionViewModel;

    public ThumbnailSelectionPage(ThumbnailSelectionViewModel vm)
    {
        InitializeComponent();

        BindingContext = vm;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        
        SetVideoViewSize();
    }

    private void SetVideoViewSize()
    {
        // if (ViewModel?.CameraOrientation is CameraOrientation.LandscapeLeft or CameraOrientation.LandscapeRight)
        // {
        //     PlayerView.VideoViewHeight = 640;
        //     PlayerView.VideoViewWidth = 480;
        // }
        // else
        // {
        //     PlayerView.VideoViewHeight = 480;
        //     PlayerView.VideoViewWidth = 640;
        // }
    }
}