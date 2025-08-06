using System.Diagnostics;
using Camera.MAUI.Ex.Models;
using Camera.MAUI.Ex.Services;

namespace Camera.MAUI.Ex.Controls;

public partial class ImageSliderBase : ContentView, INavAwareControl
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
    
    public ICameraService CameraService
    {
        get => _cameraService;
        private set
        {
            _cameraService = value;
            OnPropertyChanged();
        }
    }
    private ICameraService _cameraService;

    public ImageSliderBase()
    {
        InitializeComponent();
        
        InjectDependencies();
    }

    public virtual void InjectDependencies(IVideoService? videoService = null, ICameraService? cameraService = null)
    {
        VideoService = videoService ?? IPlatformApplication.Current.Services.GetRequiredService<IVideoService>();
        CameraService = cameraService ?? IPlatformApplication.Current.Services.GetRequiredService<ICameraService>();
    }

    public async Task LoadThumbnailsAsync()
    {
        try
        {
            Debug.WriteLine($"==== {nameof(ImageSliderBase)}.{nameof(LoadThumbnailsAsync)} - Width: {OuterBorder.Width}, Height: {OuterBorder.Height}");
            VideoService.ClearThumbnails();
            var controlSize = new Size(OuterBorder.Width, OuterBorder.Height);

            await foreach (var thumbnail in VideoService.GenerateThumbnailChain(controlSize))
            {
                Debug.WriteLine($"======{nameof(ImageSliderBase)}.{nameof(LoadThumbnailsAsync)} - Thumbnail URL: {thumbnail?.ImageUrl ?? "null"}");
                if (thumbnail != null)
                {
                    UpdateThumbnailsCollection(thumbnail);
                }
            }
        }
        catch (Exception ex)
        {
            // this was throwing when videoService.ThumbnailWidthChanged was not subscribed to, stopping thmb-generation
            // but the exception was on a task, so no stack trace was visible in the console.
            Debug.WriteLine($"======{nameof(ImageSliderBase)}.{nameof(LoadThumbnailsAsync)} - Error generating thumbnails: {ex}");
            throw;
        }
    }

    private void UpdateThumbnailsCollection(Thumbnail thumbnail)
    {
        Debug.WriteLine($"======{nameof(ImageSliderBase)}.{nameof(UpdateThumbnailsCollection)} - Thumbnail URL: {thumbnail?.ImageUrl}");
        VideoService.AddThumbnail(thumbnail);
    }
    
    public void OnNavigatedFrom(object sender, EventArgs e)
    {
        Debug.WriteLine($"==== {nameof(ImageSliderBase)}.{nameof(OnNavigatedFrom)} - Width: {OuterBorder.Width}, Height: {OuterBorder.Height}");
    }

    public void OnNavigatedTo(object sender, EventArgs e)
    {
        Debug.WriteLine($"==== {nameof(ImageSliderBase)}.{nameof(OnNavigatedTo)} - Width: {OuterBorder.Width}, Height: {OuterBorder.Height}");

        Dispatcher.DispatchAsync(async () => await LoadThumbnailsAsync());
    }
}