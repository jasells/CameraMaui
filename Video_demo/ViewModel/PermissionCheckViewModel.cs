using System.Windows.Input;
using System.Diagnostics;
using Video_Demo.Views;
using Camera.MAUI.Ex.Services;
using CommunityToolkit.Mvvm.Input;

namespace Video_Demo.ViewModel;

public class PermissionCheckViewModel : BaseViewModel
{
    private bool _permissionsGranted;
    private readonly IVideoRequirementsService _videoRequirementsService;
    public bool PermissionsGranted
    {
        get => _permissionsGranted;
        set
        {
            if (!SetProperty(ref _permissionsGranted, value)) 
                return;
            
            OnPropertyChanged();

            if (value)
            {
                NavigateToVideoRecordPage();
            }
        }
    }
    private bool _isPlaying = false;
    public bool IsPlaying
    {
        get => _isPlaying;
        set => SetProperty(ref _isPlaying, value);
    }

    public IAsyncRelayCommand CheckPermissionsCommand { get; }

    public ICommand PlayPauseCommand => new Command(OnPlayPause);

    public PermissionCheckViewModel(IVideoRequirementsService videoRequirementsService)
    {
        // can replace/override any of the commands on the service here to provide app-custom 
        // UI feedback to missing permissions/etc: MissingPermissionsCommand, InsufficientSpaceCommand
        // OR override in a derived class: HandleMissingPermissions(),HandleInsufficientSpace()
        // Just using default for now.
        _videoRequirementsService = videoRequirementsService;

        //***NOTE!!!!!  DO NOT use ICommand to wrap async methods!  Use an asyncCommand of _some_ kind!
        CheckPermissionsCommand = new AsyncRelayCommand(CheckAndHandlePermissions,
                                                        canExecute: () => !PermissionsGranted,
                                                        AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler);
        ResetPermissions();
    }

    public void ResetPermissions()
    {
        PermissionsGranted = false;
    }
    
    private void OnPlayPause() => IsPlaying = !IsPlaying;

    #region Permissions Checks
    public async Task CheckAndHandlePermissions()
    {
        try
        {
            Debug.WriteLine("Checking permissions...");
            //** note: can't find any pre-built IAsyncCommand that _returns_ a type, only IAsyncCommand that _takes_ a type(param).
            // if we want to wrap this in a command, and still return a value, we would have to define a
            // new IAsyncCommand<Tparam, Treturn>, or find a package that already does this.  Unlikely since 
            // commands are usually used for binding to UI where there is no return value.
            var (hasEnoughSpace, hasPermissions) = await _videoRequirementsService.CheckRequirementsAsync();

            // moved to default imnpl internal to service: _videoRequirementsService.MissingPermissionsCommand
            //if (!hasPermissions)
            //{
            //    PermissionsGranted = false;
            //    await HandleMissingPermissions();
            //    return;
            //}

            // moved to default impl internal to service: _videoRequirementsService.InsufficientSpaceCommand
            //if (!hasEnoughSpace)
            //{
            //    PermissionsGranted = false;
            //    await Application.Current.MainPage.DisplayAlert(
            //        "Insufficient Storage",
            //        "Please free up some space on your device before recording.",
            //        "OK");
            //    return;
            //}

            Debug.WriteLine("All permissions granted, navigating...");
            PermissionsGranted = true;
        }
        catch (Exception ex)
        {
            // note: always print out the exception to the debug console, so we can see the stack-trace in the output,
            // not just ex.Message, which is often not very useful.
            Debug.WriteLine($"Error checking permissions: {ex}");
            PermissionsGranted = false;
        }
    }

    private async void NavigateToVideoRecordPage()
    {
        try
        {
            Debug.WriteLine("Navigate to VideoRecordPage...");
            await Shell.Current.GoToAsync(nameof(VideoRecordPage));
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error navigating to VideoRecordPage: {ex}");
            await Application.Current!.MainPage!.DisplayAlert("Navigation Error", 
                "Could not navigate to the video recording page. Please try again.", "OK");
        }
    }
    #endregion
} 