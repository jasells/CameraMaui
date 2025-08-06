using System;
using Camera.MAUI;
using System.Collections.ObjectModel;
using Microsoft.Maui.Graphics;
using Camera.MAUI.Ex.Models;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Camera.MAUI.Ex.Services
{
    public interface ICameraService: INotifyPropertyChanged, INotifyPropertyChanging
    {
        Size SelectedResolution { get; set; }

        Size DefaultResolution { get; set; }

        /// <summary>
        /// For now, we only need to pick 1st mic, but if earbuds connected, may have > 1 mic? 
        /// </summary>
        MicrophoneInfo ActiveMic { get; set; }
        CameraInfo ActiveCamera { get; set; }
        ObservableCollection<CameraInfo> Cameras { get; set; }
        bool DisplayPreview { get; set; }

        /// <summary>
        /// Currently visible camera view used to control recording. This is a work-around for the
        /// poorly designed Camera.MAUI library which only exposes this via the View instead of a service
        /// the view could use.
        /// </summary>
        ICameraViewEx CameraView { get; set; }

        CameraOrientation CurrentOrientation { get; }

        Task<CameraResult> StopRecordingAsync();

        /// <summary>
        /// Calls the currently visible camera view to start recording: <see cref="CameraView"/>.
        /// </summary>
        /// <returns></returns>
        Task<CameraResult> StartRecordingAsync(string filePath, Size Resolution = default);

        void StartAccelerometer();
        void StopAccelerometer();

        //**todo: remove these events and use the INotifyPropertyChanged interface instead?
        // or, just fire them from an internal method when the property changes?
        event EventHandler<CameraInfo> CameraChanged;
        event EventHandler CamerasChanged;
        event EventHandler<bool> DisplayPreviewChanged;
        event EventHandler<Size> SelectedResolutionChanged;
    }
}
