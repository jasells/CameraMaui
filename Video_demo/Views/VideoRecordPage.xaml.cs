using Camera.MAUI.Ex.Controls;
using Video_Demo.ViewModel;

namespace Video_Demo.Views;

public partial class VideoRecordPage : BaseVideoRecordPage
{
	public VideoRecordPage()
	{
        InitializeComponent();

        // Finding the parent page on NavAwareView is not working if we add VideoRecordView as children to Grid
        var view = IPlatformApplication.Current!.Services.GetRequiredService<VideoRecordView>();
        VideoRecViewHolder.Content = view;
        
        BindingContext = IPlatformApplication.Current?.Services.GetService<VideoRecordViewModel>();
    }
}