using System.Diagnostics;
using System.Windows.Input;
using Camera.MAUI.Ex.Extensions;
using CommunityToolkit.Mvvm.Input;
using LibVLCSharp.Shared;
using Camera.MAUI.Ex.Models;
using Video_Demo.Views;
using Camera.MAUI.Ex.Services;

namespace Video_Demo.ViewModel;

public partial class TrimmingViewModel : BaseViewModel
{
    private readonly IVideoService _videoService;
    private readonly IVideoFileService _videoFileService;

    private LibVLCSharp.Shared.MediaPlayer _mediaPlayer;
    
    public LibVLCSharp.Shared.MediaPlayer MediaPlayer
    {
        get => _mediaPlayer;
        set => SetProperty(ref _mediaPlayer, value);
    }
    
    private bool _isPlaying = false;
    public bool IsPlaying
    {
        get => _isPlaying;
        set => SetProperty(ref _isPlaying, value);
    }
    
    //private string _progressVideoTime;
    //public string ProgressVideoTime
    //{
    //    get => _progressVideoTime;
    //    set => SetProperty(ref _progressVideoTime, value);
    //}
    
    private double _trimFrom;
    public double TrimFrom
    {
        get => _trimFrom;
        set
        {
            SetProperty(ref _trimFrom, value);

            StartChanged();
        }
    }
    
    private double _trimTo;
    public double TrimTo
    {
        get => _trimTo;
        set
        {
            SetProperty(ref _trimTo, value);

            EndChanged();
        }
    }
    
    public double Duration => TrimTo - TrimFrom;
    
    private string _filePath;
    public string FilePath
    {
        get => _filePath;
        set => SetProperty(ref _filePath, value);
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
    
    private ICommand _playCommand;
    public ICommand PlayCommand
    {
        get => _playCommand;
        set => SetProperty(ref _playCommand, value);
    }
    
    public ICommand NextCommand { get; set; }
    public ICommand BackCommand { get; set; }
    
    public TrimmingViewModel(
        IVideoService videoService, 
        IVideoFileService videoFileService)
    {
        _videoService = videoService;
        _videoFileService = videoFileService;

        PlayCommand = new Command(PlayControlCommand);
        NextCommand = new Command(TrimVideo);
        BackCommand = new Command(ExecuteBack);
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
        
        InitializePlayerTime();
    }

    private void InitializePlayerTime()
    {
        MediaPlayer.Play();
        TrimTo = MediaPlayer.Length;
    }

    public void StartChanged()
    {
        //TODO: Position bar to be positioned in the left thumb when it changes
        //var newStartPosition = (float)(TrimFrom/MediaPlayer.Length);
        //MediaPlayer.Position = newStartPosition;
    }
    
    public void EndChanged()
    {
        //TODO: Position bar to be positioned in the right thumb when it changes
        //var newStartPosition = (float)(TrimTo/MediaPlayer.Length);
        //MediaPlayer.Position = newStartPosition;
    }
    
    private void UpdateProgressVideoTime()
    {
        //TODO: Temporary fix. Video view is not playing full duration of the video
        // EndReached and Stopped event does not work on replaying the video
        if (!(TrimTo > 0)) return;
        
        var cutOffMs = TrimTo - MediaPlayer.Time;
        if (!(cutOffMs <= 600)) return; // most remaining ms not played based on testing is 600
        
        //TODO: Needs improvement that it should position at the very start of left TrimFrom
        // Currently, there's a little ms gap on position bar after the TrimFrom
        MediaPlayer.Position = (float)(TrimFrom/(double)MediaPlayer.Length); // Replay video
        MediaPlayer.Play();
    }
    
    private void MediaPlayerOnTimeChanged(object? sender, MediaPlayerTimeChangedEventArgs e)
    {
        UpdateProgressVideoTime();
    }
    
    
    [RelayCommand]
    private void SeekPlayer(double? args)
    {
        if (args is double position)
        {
            // Set position in Video player
            var duration = _mediaPlayer.Media.Duration;
            var frameTime = duration * position;
            _mediaPlayer.Time = (long)frameTime;
            _mediaPlayer.NextFrame();
            
            // todo: try this to see if it improves seek performance?
            //_mediaPlayer.SeekTo();

            // Set position on slider. Note: This should not perform another seek.
        }
    }

    public void PlayControlCommand()
    { 
        if (IsPlaying)
        {
            MediaPlayer.Pause();
            
            IsPlaying = false;
        }
        else
        {
            MediaPlayer.Play();
            
            IsPlaying = true;  
        }
    }

    private async void TrimVideo()
    {
        var res = await _videoService.TrimVideo(FilePath, TrimFrom, TrimTo);
        
        if (res != null)
        {
            Debug.WriteLine(res);

            NavigateToThumbnailSelection(res);
        }
        else
        {
            Debug.WriteLine("ERROR TRIMMING");
        }

    }
    
    private async void ExecuteBack()
    {
        await HandleBackNavigation();
    }
    
    private async Task HandleBackNavigation()
    {
        if (!string.IsNullOrEmpty(FilePath))
        {
            await _videoFileService.DeleteVideoFile(FilePath);
        }
        
        await Shell.Current.GoToAsync("..");
    }
    
    private async void NavigateToThumbnailSelection(string trimmedFilePath)
    {
        MediaPlayer.TimeChanged -= MediaPlayerOnTimeChanged;

        try
        {
            var navigationParameter = new ShellNavigationQueryParameters
            {
                { "FilePath", trimmedFilePath },
                { "CameraOrientation", CameraOrientation }
            };
            
            await Shell.Current.GoToAsync(nameof(ThumbnailSelectionPage), navigationParameter);
        }
        catch (Exception e) 
        { 
            Debug.WriteLine(e);
        }
    }
}