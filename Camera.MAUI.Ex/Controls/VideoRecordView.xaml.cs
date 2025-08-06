using System.ComponentModel;
using System.Diagnostics;
using Camera.MAUI.Ex.Services;

namespace Camera.MAUI.Ex.Controls;

public partial class VideoRecordView : NavAwareView
{
    private bool Initialized { get; set; }

    public ICameraService CameraService
    {
        get => _camService;
        private set
        {
            _camService = value;
            OnPropertyChanged();
        }
    }
    private ICameraService _camService;

    public IVideoRecordingService VideoRecService
    {
        get => _videoRecService;
        private set
        {
            _videoRecService = value;
            OnPropertyChanged();
        }
    }
    private IVideoRecordingService _videoRecService;
    
    private IVideoService _videoPlayerService;

    public VideoRecordView()
	{
        InitializeComponent();
    }
    
    public VideoRecordView(ICameraService cameraService,
                            IVideoRecordingService recSrv,
                            IVideoService videoPlayerSrv) : this()
    {
        InjectDependencies(cameraService, recSrv, videoPlayerSrv);
    }

    /// <summary>
    /// There are other ways to do this, possibly better... but this is the easiest way to get
    /// this view to work with a xaml layout and also inject the services... for now.
    /// </summary>
    /// <param name="cameraService"></param>
    /// <param name="recSrv"></param>
    public VideoRecordView InjectDependencies(ICameraService cameraService,
                                   IVideoRecordingService recSrv,
                                   IVideoService videoPlayerSrv)
    {
#if DEBUG
        this.PropertyChanged += (s, e) =>
        {
            Debug.WriteLine($"======= {nameof(VideoRecordView)} Property changed: {e.PropertyName}");
        };
#endif

        CameraService = cameraService;

        VideoRecService = recSrv;
        
        CountdownTimerView.InjectDependencies(recSrv);
        RecordButtonCustomView.InjectDependencies(recSrv);
        
        CameraView.AutoStartPreview = false;//make sure it is disabled so that it can toggle when cam selected

        // Replace the lambda with the instance method
        CameraView.PropertyChanged += CameraView_PropertyChanged;

        // On Android, setting to true is not mirroring using front camera - GitHub issue: https://github.com/hjam40/Camera.MAUI/issues/137
        // On iPad, setting to true will render the camera display upside down on landscape using front camera
        // On iPhone, setting to false will render the camera display upside down on landscape using front camera
        // using Back camera have different behavior also, but we are not using back camera for now, just noting here for future reference
        CameraView.MirroredImage = DeviceInfo.Platform == DevicePlatform.iOS
                                    && DeviceInfo.Idiom == DeviceIdiom.Phone;

        _videoPlayerService = videoPlayerSrv;

        return this;
    }

    private void CameraView_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        Debug.WriteLine($"======= CameraView Property changed: {e.PropertyName}");
        if (e.PropertyName == nameof(CameraView.Camera))
        {
            // this is a workaround for Maui.Camera lib design.... so that the camera is on by default,
            // if set to true in camservice
            SetAutoStartPreviewAsync();
        }
        if (e.PropertyName == nameof(CameraView.AutoStartPreview))
        {
            Debug.WriteLine($"======= CameraView AutoStartPreview: {CameraView?.AutoStartPreview}");
        }
    }

    private void SetAutoStartPreviewAsync(int delay = 10)
    {
        if (_onscreen == false) return;

        Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(delay), () =>
        {
            CameraView.AutoStartPreview = CameraService?.DisplayPreview ?? false;
        });
    }
    
    #region Appearing/Disappearing handlers
    
    // need this to guard against creating weird internal condition in cameraview if we change cams 
    // in settings page, and then come back to this page
    bool _onscreen = false;
    
    protected override void SetupParentPage(Page parent)
    {
        base.SetupParentPage(parent);

        if (!Initialized)
        {
            // improvement: could use ParentPage = null to track initialized state?
            Initialized = true;

            ((IVideoViewParent)parent).NavigatedToAsync += () =>
            {
                Debug.WriteLine($"======= {nameof(VideoRecordView)}.ParentPage NavigatedToAsync");
                return OnNavigatedTo(ParentPage, EventArgs.Empty);
            };

            ParentPage.Disappearing += (s, e) =>
            {
                Debug.WriteLine($"======= {nameof(VideoRecordView)}.ParentPage Disappearing");
                OnDisappearing(s as Page, e);
            };
        }
        //**todo/improvement: need to clean up these events when parent set = null?
    }

    /// <summary>
    /// This impl runs synchronously.
    /// <para/>
    /// Changed from OnAppearing to OnNavigatedTo to fix iOS camera initialization issues.
    /// OnAppearing was too early in the page lifecycle, causing the camera to get stuck.
    /// OnNavigatedTo ensures the page is fully ready before initializing the camera.
    /// <para/>
    /// 
    /// </summary>
    /// <param name="s"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    protected override Task OnNavigatedTo(Page s, EventArgs e)
    {
        Debug.WriteLine($"======= {nameof(VideoRecordView)} NavigatedTo");

        // pass Dispatcher to the service
        VideoRecService.Dispatcher = Dispatcher;
        
        // tell the service about which view is currently visible
        CameraService.CameraView = CameraView;
        CameraService.StartAccelerometer();

        // at least set the initial preview state, even if we can't get any updates to change it on the fly...
        // shouldn't need to now that the service has a view-ref, can just call it directly there...

        _onscreen = true; //set this before we add event handlers
        //make sure it is disabled so that it can toggle when cam selected
        CameraView.AutoStartPreview = false;
        if (CameraView.Camera != null) { SetAutoStartPreviewAsync(); }

        return Task.CompletedTask;
    }
    
    protected override async void OnDisappearing(Page s, EventArgs e)
    {
        Debug.WriteLine($"======= {nameof(VideoRecordView)} Disappearing");
        _onscreen = false;
        // in case the camera view is still recording, stop it
        await VideoRecService.StopRecordingInternal(false);

        // remove Dispatcher from the service when not on screen, avoid holding onto it
        VideoRecService.Dispatcher = null;

        // stop the accelerometer when the page is no longer visible
        CameraService.StopAccelerometer();

        // remove the ref to view no longer on screen/accessible  
        CameraService.CameraView = null;

        // have to set this to false, otherwise the camera view will not be able to start
        // if we come back to this page/change cams
        CameraView.AutoStartPreview = false;

        CameraService.PropertyChanged -= CameraView_PropertyChanged;

        // sync the file path to the video player service
        _videoPlayerService.FilePath = _videoRecService.FilePath;
        _videoPlayerService.VideoOrientation = CameraService.CurrentOrientation;
        _videoPlayerService.VideoResolution = CameraService.SelectedResolution;
    }
    
    protected override Task OnNavigatingFrom(Page page, EventArgs e)
    {
        //no impl
        return Task.CompletedTask;
    }

    protected override void OnAppearing(Page page, EventArgs e)
    {
        //no impl
    }

    #endregion
}