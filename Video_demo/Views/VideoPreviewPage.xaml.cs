using Camera.MAUI.Ex.Controls;
using Video_Demo.ViewModel;

namespace Video_Demo.Views;

public partial class VideoPreviewPage : ContentPage
{
    public VideoPreviewPage()
    {
        InitializeComponent();
        
        // Finding the parent page on NavAwareView is not working if we add VideoPreview as children to Grid
        var videoPreview = IPlatformApplication.Current!.Services.GetService<VideoPreview>();
        VideoViewHolder.Content = videoPreview;

        BindingContext = IPlatformApplication.Current?.Services.GetService<VideoPreviewViewModel>();
    }
    
    protected override bool OnBackButtonPressed()
    {
        if (BindingContext is BaseViewModel viewModel)
        {
            return viewModel.OnBackButtonPressed();
        }

        return base.OnBackButtonPressed();
    }
}