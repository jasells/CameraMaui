using CommunityToolkit.Mvvm.Input;
using Camera.MAUI.Ex.Services;
using Microsoft.Maui.Graphics;

namespace Video_Demo.ViewModel;

public partial class SettingsViewModel : BaseViewModel
{
    public ICameraService CameraService { get; set; }

    public SettingsViewModel(ICameraService cameraService)
    {
        CameraService = cameraService;
        CameraService.CameraChanged += OnCameraChanged;
    }

    private void OnCameraChanged(object sender, Camera.MAUI.CameraInfo camera)
    {
        if (camera == null) return;

        // Try to find 720p resolution first
        var targetResolution = new Size(1280, 720); // 720p
        var resolution = camera.AvailableResolutions
            .FirstOrDefault(s => s.Width == targetResolution.Width && s.Height == targetResolution.Height);

        // If 720p not found, try 1080p
        if (resolution == default)
        {
            targetResolution = new Size(1920, 1080); // 1080p
            resolution = camera.AvailableResolutions
                .FirstOrDefault(s => s.Width == targetResolution.Width && s.Height == targetResolution.Height);
        }

        CameraService.SelectedResolution = resolution;
    }

    [RelayCommand]
    private void SendTestEmail()
    {
        // go back to home tab cleanly, without resetting stack history/going to root (permissions)
        Shell.Current.CurrentItem = Shell.Current.Items.FirstOrDefault()?//find the tab bar
                                                 .Items.FirstOrDefault(x => x.Route.Contains("home"));

    }
}