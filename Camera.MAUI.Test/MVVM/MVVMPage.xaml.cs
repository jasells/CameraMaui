namespace Camera.MAUI.Test;

public partial class MVVMPage : ContentPage
{
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
}