using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Debug = System.Diagnostics.Debug;
using MauiApp = Microsoft.Maui.Controls.Application;
using CommunityToolkit.Mvvm.Input;


#if ANDROID
using Android.OS.Storage;
using Android.App;
using Android.OS;
using Application = Android.App.Application;
#endif

namespace Camera.MAUI.Ex.Services;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public class VideoRequirementsService: NotifyPropertyChangedBase, IVideoRequirementsService
{
    /// <summary>
    /// Minimum required storage space in MB for video recording. Default is 100 MB.
    /// </summary>
    public long MinimumRequiredStorageMb
    {
        get => _minSpace; set => SetProperty(ref _minSpace, value);
    }
    private long _minSpace = 100;


    public IAsyncRelayCommand MissingPermissionsCommand
    {
        get => _missingPermsCmd; set => SetProperty(ref _missingPermsCmd, value);
    }
    private IAsyncRelayCommand _missingPermsCmd;

    public IAsyncRelayCommand InsufficientSpaceCommand
    { 
        get => _insufficientSpaceCmd; set => SetProperty(ref _insufficientSpaceCmd, value);
    }
    private IAsyncRelayCommand _insufficientSpaceCmd;

    public VideoRequirementsService()
    {
        _missingPermsCmd = new AsyncRelayCommand(HandleMissingPermissions,
                                                 canExecute: () => true,
                                                 AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler);
        _insufficientSpaceCmd = new AsyncRelayCommand(HandleInsufficientSpace,
                                                     canExecute: () => true,
                                                     AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler);
    }

    public async Task<(bool hasEnoughSpace, bool hasPermissions)> CheckRequirementsAsync()
    {
        bool hasEnoughSpace = await CheckStorageSpaceAsync();
        bool hasPermissions = await CheckAllPermissionsAsync();

        if (!hasPermissions && MissingPermissionsCommand?.CanExecute(hasPermissions) == true)
        {
            await MissingPermissionsCommand.ExecuteAsync(null);
        }
        if (!hasEnoughSpace && InsufficientSpaceCommand?.CanExecute(hasPermissions) == true)
        {
            await InsufficientSpaceCommand.ExecuteAsync(null);
        }
        return (hasEnoughSpace, hasPermissions);
    }



    virtual public Task<bool> CheckStorageSpaceAsync()
    {
        //** this _might_ be why the app is crashing sometimes?
        return Task.Run(() =>
        {
#if ANDROID
            try
            {
                if (Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
                {
                    Debug.WriteLine("Storage check not supported on Android versions below 5.0 (API 21)");
                    return true; // Assume storage is available for older versions
                }

                if (Application.Context.FilesDir != null)
                {
                    var stat = new StatFs(Application.Context.FilesDir.AbsolutePath);
                    var availableBlocks = stat.AvailableBlocksLong;
                    var blockSize = stat.BlockSizeLong;
                    var availableBytes = availableBlocks * blockSize;
                    var availableMb = availableBytes / (1024 * 1024);

                    Debug.WriteLine($"Available storage: {availableMb}MB");
                    return availableMb >= MinimumRequiredStorageMb;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking storage space: {ex.Message}");
                return false;
            }
#elif IOS
            try
            {
                var fileManager = Foundation.NSFileManager.DefaultManager;
                var attributes = fileManager.GetFileSystemAttributes(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
                var availableBytes = attributes.FreeSize;
                var availableMB = availableBytes / (1024 * 1024);

                Debug.WriteLine($"Available storage: {availableMB}MB");
                return availableMB >= (ulong)MinimumRequiredStorageMb;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking storage space: {ex.Message}");
                return false;
            }
#else
            return true;
#endif
            
            return false;
        });
    }

    virtual public async Task<bool> CheckAllPermissionsAsync()
    {
        try
        {
            var cameraStatus = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (cameraStatus != PermissionStatus.Granted)
            {
                cameraStatus = await Permissions.RequestAsync<Permissions.Camera>();
                if (cameraStatus != PermissionStatus.Granted)
                {
                    Debug.WriteLine("Camera permission denied.");
                    return false;
                }
            }

            var microphoneStatus = await Permissions.CheckStatusAsync<Permissions.Microphone>();
            if (microphoneStatus != PermissionStatus.Granted)
            {
                microphoneStatus = await Permissions.RequestAsync<Permissions.Microphone>();
                if (microphoneStatus != PermissionStatus.Granted)
                {
                    Debug.WriteLine("Microphone permission denied.");
                    return false;
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error checking permissions: {ex.Message}");
            return false;
        }
    }

    // make a command for this.
    virtual public async Task<bool> OpenAppSettingsAsync()
    {
#if ANDROID
        if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
        {
            var intent = new Android.Content.Intent(Android.Provider.Settings.ActionApplicationDetailsSettings);
            intent.AddFlags(Android.Content.ActivityFlags.NewTask);
            var uri = Android.Net.Uri.FromParts("package", Application.Context.PackageName, null);
            intent.SetData(uri);
            Application.Context.StartActivity(intent);
        }
        else
        {
            Debug.WriteLine("Settings intent not supported on Android versions below 5.0 (API 21)");
        }
#elif IOS
        AppInfo.Current.ShowSettingsUI();
#else
        System.Diagnostics.Debug.WriteLine("Opening app settings not supported on this platform");
#endif

        await Task.Delay(1000);
        
        return await CheckAllPermissionsAsync();
    }

    //** todo: make commands for showing error messages when permissions are not granted or storage is insufficient.
    virtual protected async Task HandleMissingPermissions()
    {
        var result = await MauiApp.Current!.MainPage.DisplayAlert("Permissions Required",
                                "Camera and microphone permissions are required to record video. Would you like to open settings?",
                                "Open Settings",
                                "Cancel");

        if (result)
        {
            await OpenAppSettingsAsync();
        }
    }

    virtual protected Task HandleInsufficientSpace() => 
                                MauiApp.Current?.MainPage.DisplayAlert( "Insufficient Storage",
                                            $"Please free up {MinimumRequiredStorageMb} MB on your device before recording.",
                                            "OK");

}
