using Camera.MAUI.Ex.Controls;
using Camera.MAUI.Ex.Models;
using Camera.MAUI;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.Input;

namespace Camera.MAUI.Ex.Services;

public class CameraService : NotifyPropertyChangedBase, ICameraService 
{
    private Size _selectedResolution = new();
    public Size SelectedResolution
    {
        get => _selectedResolution;
        set
        {
            if(SetProperty(ref _selectedResolution, value))
            {
                SelectedResolutionChanged?.Invoke(this, _selectedResolution);
            }
        }
    }

    /// <summary>
    /// Default value is 1280x720 (720p). This is used to set the default resolution when a camera is selected,
    /// unless <see cref="SelectedResolution"/> is set to a different value.
    /// </summary>
    public Size DefaultResolution
    {
        get => _defaultResolution;
        set => SetProperty(ref _defaultResolution, value);
    }
    private Size _defaultResolution = new(1280, 720); // default to 720p

    public CameraInfo? ActiveCamera
    {
        get => _camera;
        set
        {
            if (value == null) { return; }
            if (SetProperty(ref _camera, value))
            {
                CameraChanged?.Invoke(this, _camera);
 
                if (ActiveCamera == null) { return; }
                
                // keep the previously selected resolution, if possible
                var sizeSelect = SelectedResolution != default
                    ? SelectedResolution
                    : DefaultResolution;
                
                SelectedResolution = ActiveCamera.AvailableResolutions
                    .FirstOrDefault(s => s == sizeSelect);
            }
        }
    }
    private CameraInfo _camera = null;


    private ObservableCollection<CameraInfo> _cameras = null;
    public ObservableCollection<CameraInfo> Cameras 
    {
        get => _cameras;
        set
        {
            if(SetProperty(ref _cameras, value))
            {
                SetDefaultCamAndMic();
            }
        }
    }

    public bool DisplayPreview 
    { 
        get => _displayPreview;
        set
        {
            if (SetProperty(ref _displayPreview, value))
            {
                DisplayPreviewChanged?.Invoke(this, _displayPreview);
            } 
        }   
    }
    private bool _displayPreview = true;

    public ICameraViewEx CameraView
    {
        get { return _camView; }
        set
        {
            if (SetProperty(ref _camView, value))
            {
                SetDefaultCamAndMic();

                if (CameraView?.Cameras != null)
                {
                    CameraView.Cameras.CollectionChanged += OnCameraListChanged;
                }
            }
        }
    }
    private ICameraViewEx _camView;


    public CameraOrientation CurrentOrientation
    {
        get => _currentOrientation;
        set => SetProperty(ref _currentOrientation, value);
    }
    private CameraOrientation _currentOrientation;


    public MicrophoneInfo ActiveMic
    { 
        get => _mic;
        set => SetProperty(ref _mic, value);
    }
    private MicrophoneInfo _mic;
    
    public event EventHandler<CameraInfo> CameraChanged;
    public event EventHandler CamerasChanged;
    public event EventHandler<bool> DisplayPreviewChanged;
    public event EventHandler<Size> SelectedResolutionChanged;

    public CameraService() {}

    public Task<CameraResult> StopRecordingAsync()
    {
        if (CameraView != null)
        {
            return CameraView.StopCameraAsync();
        }
        else
        {
            // not sure if this is best, but will work for now.
            return Task.FromResult(CameraResult.NoCameraSelected);
        }
    }

    public Task<CameraResult> StartRecordingAsync(string file, Size resolution = default(Size))
    {     
        if (CameraView != null)
        {
            StopAccelerometer();
            resolution = resolution == default(Size)
                        ? SelectedResolution
                        : DefaultResolution;
            return CameraView.StartRecordingAsync(file, resolution);
        }
        else
        {
            // not sure if this is best, but will work for now.
            return Task.FromResult(CameraResult.NoCameraSelected);
        }
    }

    public void StopAccelerometer()
    {
        if (Accelerometer.IsSupported)
        {
            Accelerometer.ReadingChanged -= OnAccelerometerReadingChanged;
            Accelerometer.Stop();
        }
    }

    public void StartAccelerometer()
    {
        if (!Accelerometer.IsSupported) return;
        if (Accelerometer.IsMonitoring) return; // already started

        Accelerometer.ReadingChanged += OnAccelerometerReadingChanged;
        Accelerometer.Start(SensorSpeed.UI);
    }

    protected override void OnPropertyChanging([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanging(propertyName);

        if (propertyName == nameof(CameraView))
        {
            if (CameraView?.Cameras != null)
            {
                CameraView.Cameras.CollectionChanged -= OnCameraListChanged;
            }
        }
    }

    private void OnCameraListChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            SetDefaultCamAndMic();
        }
    }

    private void OnAccelerometerReadingChanged(object sender, AccelerometerChangedEventArgs e)
    {
        var reading = e.Reading;
        double x = reading.Acceleration.X;
        double y = reading.Acceleration.Y;

        if (Math.Abs(y) > Math.Abs(x))
        {
            CurrentOrientation = y > 0 ? CameraOrientation.Portrait : CameraOrientation.UpsideDown;
        }
        else
        {
            CurrentOrientation = x > 0 ? CameraOrientation.LandscapeLeft : CameraOrientation.LandscapeRight;
        }
    }

    private void SetDefaultCamAndMic()
    {
        // we don't want to null-out the collection, cause a crash if UI is bound to it
        if (CameraView != null) Cameras = CameraView?.Cameras;
        ActiveCamera ??= Cameras?.FirstOrDefault(cam => cam.Position == CameraPosition.Front);
        
        //For now, we only need to pick 1st mic, but if earbuds connected, may have > 1 mic? 
        ActiveMic ??= CameraView?.Microphones.FirstOrDefault();
    }
}