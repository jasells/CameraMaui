using Camera.MAUI.Ex.Services;

namespace Camera.MAUI.Ex.Controls
{
    public partial class CountdownTimerView : ContentView
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

        public CountdownTimerView()
        {
            InitializeComponent();
        }
        
        public void InjectDependencies(IVideoRecordingService videoRecordingService)
        {
            VideoRecService = videoRecordingService;
        }
    }
}
