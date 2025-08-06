using Camera.MAUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camera.MAUI.Ex.Services;

/// <summary>
/// Allows for better access to and control of the CameraView class in the Camera.MAUI library via the <see cref="CameraService"/>.
/// </summary>
public interface ICameraViewEx : ICameraView
{
    ///// <summary>
    ///// The camera view is currently recording.
    ///// </summary>
    //bool IsRecording { get; set; }
    ///// <summary>
    ///// The camera view is currently previewing.
    ///// </summary>
    //bool IsPreviewing { get; set; }
    /// <summary>
    /// The camera view is currently mirroring the image.
    /// </summary>
    bool MirroredImage { get; set; }

    Task<CameraResult> StartRecordingAsync(string file, Size Resolution = default);

    Task<CameraResult> StopCameraAsync();

    int NumCamerasDetected { get; }

    ObservableCollection<CameraInfo> Cameras { get; }

    int NumMicrophonesDetected { get; }

    ObservableCollection<MicrophoneInfo> Microphones { get; }

    bool AutoStartPreview { get; set; } 
    

}

