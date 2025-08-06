using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Camera.MAUI.Ex.Services;
using Camera.MAUI;
using System.Collections.ObjectModel;

namespace Camera.MAUI.Ex.Controls;

/// <summary>
/// Allows for better access to and control of the CameraView class in the Camera.MAUI library via the <see cref="Camera.MAUI.Ex.Services.CameraService"/>.
/// </summary>
public class CameraViewEx : Camera.MAUI.CameraView, Services.ICameraViewEx
{
    public bool MirroredImage
    {
        get => base.MirroredImage;
        set => base.MirroredImage = value;
    }

    public Task<CameraResult> StartRecordingAsync(string file, Size Resolution = default)
        => base.StartRecordingAsync(file, Resolution);

    public Task<CameraResult> StopCameraAsync()
        => base.StopRecordingAsync();

    public int NumCamerasDetected => base.NumCamerasDetected;

    public ObservableCollection<CameraInfo> Cameras => base.Cameras;

    public int NumMicrophonesDetected => base.NumMicrophonesDetected;

    public ObservableCollection<MicrophoneInfo> Microphones => base.Microphones;

    public bool AutoStartPreview
    {
        get => base.AutoStartPreview;
        set => base.AutoStartPreview = value;
    }
}
