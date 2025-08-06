using Camera.MAUI.Ex.Services;

namespace Camera.MAUI.Ex.Controls;

public partial class RecordButtonCustomView : ContentView
{
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
    
    public RecordButtonCustomView()
    {
        InitializeComponent();
    }
    
    public void InjectDependencies(IVideoRecordingService videoRecordingService)
    {
        VideoRecService = videoRecordingService;
    }
}