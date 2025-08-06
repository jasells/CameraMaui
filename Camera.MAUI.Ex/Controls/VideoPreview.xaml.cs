using Camera.MAUI.Ex.Services;
using System.Diagnostics;

namespace Camera.MAUI.Ex.Controls;

public partial class VideoPreview : NavAwareView
{
    public IVideoService VideoService
    {
        get => _videoService;
        private set
        {
            _videoService = value;
            OnPropertyChanged();
        }
    }
    private IVideoService _videoService;
    
    public VideoPreview()
    {
        InitializeComponent();
    }

    public VideoPreview(ICameraService cameraService,
                        IVideoService videoSvc) : this()
    {
        InjectDependencies(cameraService, videoSvc);
    }

    /// <summary>
    /// There are other ways to do this, possibly better... but this is the easiest way to get
    /// this view to work with a xaml layout and also inject the services... for now.
    /// </summary>
    /// <param name="cameraService"></param>
    ///  <param name="videoSvc"></param>
    public VideoPreview InjectDependencies(ICameraService cameraService,
                                   IVideoService videoSvc)
    {
#if DEBUG
        this.PropertyChanged += (s, e) =>
        { 
            Debug.WriteLine($"======= {nameof(VideoPreview)} Property changed: {e.PropertyName}");
        };
#endif
        //**todo: this needs to be contained in the VideoPlayerView only to reduce the coupling.
        VideoService = videoSvc;
        Slider.InjectDependencies(_videoService);
        PlayerView.InjectDependencies(_videoService, cameraService);

        return this;
    }
    
    protected override void SetupParentPage(Page parent)
    {
        base.SetupParentPage(parent);
        
        ParentPage.NavigatedTo += (s, e) =>
        {
            OnNavigatedTo(s as Page, e);
        };
        
        ParentPage.NavigatingFrom += (s, e) =>
        {
            OnNavigatingFrom(s as Page, e);
        };
    }

    protected override Task OnNavigatedTo(Page page, EventArgs e)
    {
        Debug.WriteLine($"======= VideoPreview OnNavigatedTo: {Width}x{Height}");

        //propagate down to child controls...
        this.Slider.OnNavigatedTo(page, e);
        
        return Task.CompletedTask;
    }

    protected override Task OnNavigatingFrom(Page page, EventArgs e)
    {
        Debug.WriteLine($"======= VideoPreview OnNavigatingFrom: {Width}x{Height}");

        _videoService.CurrentPlayer?.Stop();

        //propagate down to child controls...
        Slider.OnNavigatedFrom(page, e);
        
        return Task.CompletedTask;
    }

    protected override void OnDisappearing(Page page, EventArgs e)
    {
        
    }

    protected override void OnAppearing(Page page, EventArgs e)
    {
        
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        
        Debug.WriteLine($"======= VideoPreview OnSizeAllocated: {width}x{height}");
    }
}