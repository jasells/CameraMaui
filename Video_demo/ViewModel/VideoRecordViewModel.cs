using File = System.IO.File;
using System.Diagnostics;
using Camera.MAUI.Ex.Resources;
using CommunityToolkit.Mvvm.Input;
using Video_Demo.Views;
using Size = Microsoft.Maui.Graphics.Size;
using Camera.MAUI.Ex.Services;
using Colors = Microsoft.Maui.Graphics.Colors;

namespace Video_Demo.ViewModel;

public partial class VideoRecordViewModel : BaseViewModel
{
    private readonly ICameraService _cameraService;
    private readonly IVideoRecordingService _vidService;

    public VideoRecordViewModel(ICameraService cameraService,
                                IVideoRecordingService vidService)
    {
        _cameraService = cameraService;
        _vidService = vidService;
        
        // values are same with defaults but set them again here
        // to easily know the fields that we can modify related to UI
        // when integrating to Product app
        _vidService.TimerFontSize = 16;
        _vidService.StartRecordingMessage = "Hit the \"record\" button to start your video.";
        _vidService.OngoingRecordingMessage = "Video recording in progress.";
        _vidService.RecordingMessageTextColor = Color.FromArgb("#FFFFFF");
        _vidService.RecordingMessageBackgroundColor = Color.FromArgb("#80000000");
        _vidService.RecordingMessageFontSize = 14d;
        _vidService.RecordingMessagePadding = new Thickness(20, 10, 20, 10);
        _vidService.HeaderBackgroundColor = Colors.Purple;
        _vidService.RecordViewBackgroundColor = Colors.Black;
        _vidService.ProgressColor = Colors.Aqua;
        _vidService.ProgressHeight = 20d;
        _vidService.TimerDefaultTextColor = Colors.Blue;
        _vidService.TimerDefaultBackgroundColor = Colors.Red;
        _vidService.TimerEndingTextColor = Colors.Yellow;
        _vidService.TimerEndingBackgroundColor = Colors.Black;
        _vidService.TimerLabelPadding = new Thickness(4,2,4,2);
        _vidService.CountdownTimerViewPadding = new Thickness(20, 10, 20, 10);
        _vidService.CountdownTimerViewSpacing = 20d;
        _vidService.RecordButtonSize = 72d;
        _vidService.RecordButtonOutlineColor = Colors.Yellow;
        _vidService.RecordButtonOutlineWidth = 4d;
        _vidService.PlayIconColor = Colors.Blue;
        _vidService.StopIconColor = Colors.Red;
        _vidService.RecordButtonMargin = new Thickness(0,0,0,40);
        _vidService.StartRecordingBottomSpacing = 10d;
        _vidService.TimerFontFamily = FontNames.PromptRegular;
        _vidService.RecordingMessageFontFamily = FontNames.PromptRegular;
        
        SetupRecordCompleteCommand();
        SetupStartRecordingCommand();
    }

    private void SetupStartRecordingCommand()
    {
        //we HAVE to hold onto the original command here, otherwise we make a recursive loop 
        // with no exit (stack overflow/segfault)
        if (_oldCommand == null)
        {
            _oldCommand = _vidService.StartRecordingCommand;
            _vidService.StartRecordingCommand = new AsyncRelayCommand<string?>((file) =>
            {
                // Try to find 720p resolution first
                var targetResolution = new Size(1280, 720); // 720p
                var resolution = _cameraService.ActiveCamera.AvailableResolutions
                                               .FirstOrDefault(s => s.Equals(targetResolution));

                // If 720p not found, try 1080p
                if (resolution == default)
                {
                    targetResolution = new Size(1920, 1080); // 1080p
                    resolution = _cameraService.ActiveCamera.AvailableResolutions
                                               .FirstOrDefault(s => s.Equals(targetResolution));
                }

                _cameraService.SelectedResolution = resolution;

                // must call the original command to avoid recursion
                // or re-create original impl here.   This causes seg-fault on android, since
                // infinite recursion is detected by runtime, or is unhandled?
                return _oldCommand.ExecuteAsync(file);
            });
        }
    }

    private static IAsyncRelayCommand? _oldCommand = null;


    //async Task StartRecording(string? file)
    //{
    //    // Try to find 720p resolution first
    //    var targetResolution = new Size(1280, 720); // 720p
    //    var resolution = _cameraService.ActiveCamera.AvailableResolutions
    //        .FirstOrDefault(s => s.Width == targetResolution.Width && s.Height == targetResolution.Height);

    //    // If 720p not found, try 1080p
    //    if (resolution == default)
    //    {
    //        targetResolution = new Size(1920, 1080); // 1080p
    //        resolution = _cameraService.ActiveCamera.AvailableResolutions
    //            .FirstOrDefault(s => s.Width == targetResolution.Width && s.Height == targetResolution.Height);
    //    }

    //    _cameraService.SelectedResolution = resolution;

    //// must call the original command to avoid recursion
    //// or re-create original impl here.  This causes seg-fault on android, since
    /// the infinite recursion is detected by runtime?
    //    await _vidService.StartRecordingCommand.ExecuteAsync(file);
    //}

    #region Camera.MAUI AutoRecord
    /// <summary>
    /// jml 10/21/2024: I discovered something ugly about Camera.MAUI and camera resolutions. Even though I can detect all resolutions from the CameraInfo class and 
    /// record them with  StartRecordingAsync(string file, Size Resolution = default(Size)) , using AutoRecord from Xaml (i.e. my MVVM solution) 
    /// does not allow manually changing resolutions. Instead, it seems to be a standard procedure to implement the use of a ZoomFactor variable 
    /// to implement an internal resolution selector. ZoomFactor has values of 1-8 (seen in my testing) and is recommended to be used with a 
    /// slider control.
    /// 
    /// This region is a workaround for that
    /// </summary>

    public Size SelectedResolution
    {
        get => _cameraService.SelectedResolution;
        set => _cameraService.SelectedResolution = value;
    }

    private void SetupRecordCompleteCommand()
    {
        _vidService.RecordingComplete = new AsyncRelayCommand(RecordingComplete);
    }

    private async Task RecordingComplete()
    {
        if (File.Exists(_vidService.FilePath))
        {
            Debug.WriteLine($"Recording saved successfully at path: {_vidService.FilePath}");

            try
            {
                await Shell.Current.GoToAsync(nameof(VideoPreviewPage));
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error navigating to VideoPreviewPage: {e.Message}");
            }
        }
        else
        {
            Debug.WriteLine("Recording not saved - file does not exist");
        }
    }

    #endregion Camera.MAUI AutoRecord
}