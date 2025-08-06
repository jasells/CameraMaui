using Camera.MAUI;
using CommunityToolkit.Maui.Views;
using Video_Demo.ViewModel;
using Camera.MAUI.Ex.Services;

#if ANDROID
using Android.OS;
#endif

namespace Video_Demo.Views;

public partial class SettingsPage : ContentPage
{
	private readonly SettingsViewModel _viewModel;

	public SettingsPage(ICameraService cameraService)
	{
		InitializeComponent();
		_viewModel = new SettingsViewModel(cameraService);
		BindingContext = _viewModel;
	}
}
