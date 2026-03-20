namespace Camera.MAUI.Test;

public partial class MVVMPage : ContentPage
{
	protected CameraViewModel? ViewModel => BindingContext as CameraViewModel;

	public MVVMPage()
	{
		InitializeComponent();
	}

	protected override void OnBindingContextChanged()
	{
		base.OnBindingContextChanged();

		if (BindingContext is CameraViewModel vm)
		{
			vm.StopCamera = new CommunityToolkit.Mvvm.Input.AsyncRelayCommand(async () => await cameraView.StopCameraAsync());
			vm.StartCamera = new CommunityToolkit.Mvvm.Input.AsyncRelayCommand(async () => await cameraView.StartCameraAsync());
		
		}
	}

	protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		base.OnNavigatedTo(args);

		// now switch to front (selfie) cam by default, since we already loaded rear cam and avoid
		// iOS issue: https://xdevapps.visualstudio.com/DefaultCollection/XDev.Maui/_workitems/edit/164
		if (DeviceInfo.Current.Platform == DevicePlatform.iOS
			&& ViewModel?.Cameras.Count > 1)
		{
			ViewModel.Camera = ViewModel.Cameras.Skip(1).First();
		}
	}
}