using Camera.MAUI.Ex.Models;
using Video_Demo.ViewModel;
using Camera.MAUI.Ex.Services;

namespace Video_Demo.Views;

public partial class TrimmingPage
{
    public TrimmingViewModel? ViewModel => BindingContext as TrimmingViewModel;
    
    public TrimmingPage(TrimmingViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        if (Slider != null)
        {
            Slider.SeekPlayer += SeekPlayer;
        }
        
        SetVideoViewSize();

        base.OnNavigatedTo(args);
    }

    private void SeekPlayer(object? sender, double e) =>
        ViewModel?.SeekPlayerCommand?.Execute(Slider?.SeekCommandParameter);

    protected override void OnDisappearing()
    {
        if (Slider != null)
        {
            //clean up
            Slider.SeekPlayer -= SeekPlayer;
        }

        base.OnDisappearing();
    }
    
    private async void OnStartOverClicked(object sender, EventArgs e)
    {
        var confirm = await DisplayAlert(
            "Title",
            "Message",
            "Accept",
            "Cancel");
        if (!confirm) return;
        
        if (ViewModel?.BackCommand?.CanExecute(null) == true)
            ViewModel.BackCommand.Execute(null);
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