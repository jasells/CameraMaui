using Camera.MAUI.Ex.Models;
using Camera.MAUI.Ex.Services;
using CommunityToolkit.Mvvm.Input;
using Video_Demo.Views;

namespace Video_Demo.ViewModel;

public class VideoPreviewViewModel : BaseViewModel
{
    private readonly IVideoFileService _videoFileService;
    private readonly IVideoRecordingService _videoRecordingService;
    
    public AsyncRelayCommand NextCommand => new AsyncRelayCommand(OnNext);

    public AsyncRelayCommand BackCommand => new AsyncRelayCommand(OnBack);

    public VideoPreviewViewModel(IVideoFileService videoFileService,
                                 IVideoRecordingService videoRecordingService,
                                 IVideoService videoPreviewService)
    {
        _videoFileService = videoFileService;
        _videoRecordingService = videoRecordingService;

        // values are same with defaults but set them again here
        // to easily know the fields that we can modify related to UI
        // when integrating to Product app
        videoPreviewService.VideoViewHeight = 480d;
        videoPreviewService.VideoViewWidth = 640d;
        videoPreviewService.TimestampFontSize = 14d;
        videoPreviewService.PlayerBorderMargin = new Thickness(20,10);
        videoPreviewService.PlayPauseIconSize = 40d;
        videoPreviewService.ViewBackgroundColor = Camera.MAUI.Ex.Resources.Colors.Snow;
        videoPreviewService.PlayerBackgroundColor = Camera.MAUI.Ex.Resources.Colors.Snow;
        videoPreviewService.TimestampBackgroundColor = Color.FromArgb("#80000000");
        videoPreviewService.TimestampTextColor = Camera.MAUI.Ex.Resources.Colors.Snow;
        videoPreviewService.TimestampFontFamily = Camera.MAUI.Ex.Resources.FontNames.PromptRegular;
        videoPreviewService.TimestampMargin = new Thickness(16,0,0,16);
        videoPreviewService.TimestampPadding = new Thickness(4,3,4,3);
        videoPreviewService.ThumbnailHeight = 40d;
        videoPreviewService.PositionBarHeight = 40d;
        videoPreviewService.SliderPositionBarColor = Camera.MAUI.Ex.Resources.Colors.Blackbird;
        videoPreviewService.SliderPositionBarStartingPoint = 0;
        videoPreviewService.SliderPositionBarWidth = 3d;
        videoPreviewService.SliderBaseMargin = new Thickness(16,8,16,8);
    }

    public override bool OnBackButtonPressed()
    {
        BackCommand.ExecuteAsync(null);
        return true;
    }

    private async Task OnNext()
    {
        var navigationParameter = new ShellNavigationQueryParameters
        {
            { "FilePath", _videoRecordingService.FilePath },
            { "CameraOrientation", CameraOrientation.Portrait} //should put this on videoService when done recording?
        };
       
        // ToDo fix crash issue when navigating to ThumbnailSelectionPage
        await Shell.Current.GoToAsync(nameof(UploadTestPage));
    }
    
    private async Task OnBack()
    {
        // Use popup page on Product app
        if (Application.Current == null || Application.Current.MainPage == null) return;
        var confirm = await Application.Current.MainPage.DisplayAlert("Are you sure you want to close this page?",
                                                                    "Closing this page will delete the video recording.",
                                                                    "Accept",
                                                                    "Cancel");
        
        if (!confirm) return;
        
        await _videoFileService.DeleteVideoFile(_videoRecordingService.FilePath);
        await Shell.Current.GoToAsync("..");
    }
}