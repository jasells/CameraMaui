using Camera.MAUI.Ex.Controls;
using Video_Demo.ViewModel;

namespace Video_Demo.Views;

public partial class UploadTestPage : ContentPage
{
	public UploadTestPage()
	{
		InitializeComponent();

		progressHolder.Content = IPlatformApplication.Current?.Services.GetService<UploadProgress>();

        //BindingContext = IPlatformApplication.Current?.Services.GetService<UploadTestViewModel>();
    }
}