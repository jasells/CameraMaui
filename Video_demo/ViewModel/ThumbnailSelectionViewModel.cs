using System.Windows.Input;
using Camera.MAUI.Ex.Extensions;
using CommunityToolkit.Mvvm.Input;
using Camera.MAUI.Ex.Models;

using System.Diagnostics;
using Camera.MAUI.Ex.Services;
using System.ComponentModel;

namespace Video_Demo.ViewModel;

public partial class ThumbnailSelectionViewModel : BaseViewModel
{
    // Test values used in CRM endpoints
    const long ConstituentId = 29696; // valid constituent for testing
    const long DonationId = 38932;


    private readonly IVideoService _videoService;

    public IAsyncRelayCommand UploadVideoCommand { get; set; }

    private bool _isProcessing;
    public bool IsProcessing
    {
        get => _isProcessing;
        set => SetProperty(ref _isProcessing, value);
    }

    private bool _displayVideo;
    public bool DisplayVideo
    {
        get => _displayVideo;
        set => SetProperty(ref _displayVideo, value);
    }

    private string _filePath;
    public string FilePath
    {
        get => _filePath;
        set => SetProperty(ref _filePath, value);
    }

    private string _videoUrl;
    public string VideoUrl
    {
        get => _videoUrl;
        set => SetProperty(ref _videoUrl, value);
    }

    private CameraOrientation _cameraOrientation;
    public CameraOrientation CameraOrientation
    {
        get => _cameraOrientation;
        set => SetProperty(ref _cameraOrientation, value);
    }

    private double _videoViewHeight = 480d;
    public double VideoViewHeight
    {
        get => _videoViewHeight;
        set => SetProperty(ref _videoViewHeight, value);
    }

    private double _videoViewWidth = 640d;
    public double VideoViewWidth
    {
        get => _videoViewWidth;
        set => SetProperty(ref _videoViewWidth, value);
    }

    private LibVLCSharp.Shared.MediaPlayer _mediaPlayer;

    public LibVLCSharp.Shared.MediaPlayer MediaPlayer
    {
        get => _mediaPlayer;
        set => SetProperty(ref _mediaPlayer, value);
    }

    //ApiVideoHelper ApiVideoApi { get; set; }
    //IVideoHostService ApiVideoApiClient { get; set; }

    [RelayCommand]
    private void SeekPlayer(object args)
    {
        if (args is double position)
        {
            // Set position in Video player
            var duration = _mediaPlayer.Media.Duration;
            var frameTime = duration * position;
            _mediaPlayer.Time = (long)frameTime;
            _mediaPlayer.NextFrame();
        }
    }

    private Thumbnail _selectedThumbnail;
    public Thumbnail SelectedThumbnail
    {
        get => _selectedThumbnail;
        set => SetProperty(ref _selectedThumbnail, value);
    }

    public ThumbnailSelectionViewModel(IVideoService videoService)
    {
        _videoService = videoService;

        UploadVideoCommand = new AsyncRelayCommand(UploadVideo);

        _videoService.ThumbnailGenerated += _videoService_ThumbnailGenerated;

        DisplayVideo = false;
        IsProcessing = false;

    }

    protected override void PopulateQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue(nameof(CameraOrientation), out var cameraOrientation))
        {
            CameraOrientation = cameraOrientation.ToString().ToCameraOrientation();
        }

        if (query.TryGetValue(nameof(FilePath), out var filepath))
        {
            FilePath = filepath.ToString();
        }
    }

    private void _videoService_ThumbnailGenerated(object? sender, string e)
    {
        SelectedThumbnail = new Thumbnail { ImageUrl = e };
    }

    public async System.Threading.Tasks.Task LoadThumbnailsAsync()
    {
        DateTime thumbGenStart = DateTime.Now;

        //var videoTrack = MediaPlayer.Media.Tracks.FirstOrDefault(x => x.TrackType == LibVLCSharp.Shared.TrackType.Video);
        //var width = videoTrack.Data.Video.Width;
        //var height = videoTrack.Data.Video.Height;

        await _videoService.GenerateThumbnail(720, 1280, 0);
    }


    private async void LoadThumbnails()
    {
        await LoadThumbnailsAsync();
    }



    private async Task UploadVideo()
    {
        IsProcessing = true;

        //var res = RecordedResolution;
        //var video = await UploadVideoWithTokenAsync();
        await Shell.Current.GoToAsync("..", true);

    }
}