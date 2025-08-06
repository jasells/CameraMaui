using System.Collections.ObjectModel;
using Camera.MAUI.Ex.Models;
using System.ComponentModel;
using System.Windows.Input;

namespace Camera.MAUI.Ex.Services;

public interface IVideoService: INotifyPropertyChanged, INotifyPropertyChanging
{
    LibVLCSharp.Shared.MediaPlayer CurrentPlayer { get; set; }
    
    string FilePath { get; set; }

    event EventHandler<string> ThumbnailGenerated;
    event EventHandler<double> ThumbnailWidthChanged;

    Task<string> TrimVideo(string videoPath, double start, double end);

    Task<Thumbnail?> GenerateThumbnail(double height, double width, long timeMs);

    /// <summary>
    /// Uses the <see cref="CurrentPlayer"/> internally to generate thumbnails for the video.
    /// </summary>
    /// <param name="orientation">orientation of the video</param>
    /// <param name="sliderSize">size on screen to hold the "chain" of thumbnails, typically a slider, like <see cref="Controls.VideoPreviewSlider"/></param>
    /// <returns></returns>
    IAsyncEnumerable<Thumbnail?> GenerateThumbnailChain(Size sliderSize);

    /// <summary>
    /// updates <see cref="CurrentMediaPlayer"/> property to allow for playback control.
    /// </summary>
    /// <param name="videoPath"></param>
    /// <param name="isVideoPreview"></param>
    /// <param name="orientation"></param>
    /// <returns></returns>
    LibVLCSharp.Shared.MediaPlayer CreateMediaPlayer(string videoPath, bool isVideoPreview, CameraOrientation orientation);

    Command<double> SeekPlayer { get; }
    Size? VideoSize { get; set; }
    ICommand PlayCommand { get; set; }
    IObservable<float> PositionEvents { get; }
    CameraOrientation VideoOrientation { get; set; }
    Size VideoResolution { get; set; }
    /// <summary>
    /// Height of the VideoView
    /// </summary>
    double VideoViewHeight { get; set; }
    /// <summary>
    /// Width of the VideoView
    /// </summary>
    double VideoViewWidth { get; set; }
    /// <summary>
    /// Width of the thumbnail images used in the slider.
    /// </summary>
    double ThumbnailWidth { get; set; }
    
    /// <summary>
    /// Height of the thumbnail images used in the slider.
    /// </summary>
    double ThumbnailHeight { get; set; }
    /// <summary>
    /// Font size for the timestamp text in the video player.
    /// </summary>
    double TimestampFontSize { get; set; }
    /// <summary>
    /// Border margin for video player container
    /// </summary>
    Thickness PlayerBorderMargin { get; set; }
    /// <summary>
    /// Size of the play/pause button in the video player.
    /// </summary>
    double PlayPauseIconSize { get; set; }
    /// <summary>
    /// Background color for the video player view.
    /// </summary>
    Color ViewBackgroundColor { get; set; }
    /// <summary>
    /// Background color for the video player container
    /// </summary>
    Color PlayerBackgroundColor { get; set; }
    /// <summary>
    /// Background color for the timestamp text in the video player.
    /// </summary>
    Color TimestampBackgroundColor { get; set; }
    /// <summary>
    /// Text color for the timestamp text in the video player.
    /// </summary>
    Color TimestampTextColor { get; set; }
    /// <summary>
    /// Font family for the timestamp text in the video player.
    /// </summary>
    string TimestampFontFamily { get; set; }
    /// <summary>
    /// margin around the timestamp text in the video player.
    /// </summary>
    Thickness TimestampMargin { get; set; }
    /// <summary>
    /// Padding around the timestamp text in the video player.
    /// </summary>
    Thickness TimestampPadding { get; set; }
    /// <summary>
    /// Height of the position bar on slider that shows the video position.
    /// </summary>
    double PositionBarHeight { get; set; }
    
    Color SliderPositionBarColor { get; set; }
    
    double SliderPositionBarStartingPoint { get; set; }
    
    double SliderPositionBarWidth { get; set; }
    
    Thickness SliderBaseMargin { get; set; }
    
    IReadOnlyCollection<Thumbnail> ThumbnailItems { get; }

    void ClearThumbnails();
    
    void AddThumbnail(Thumbnail thumbnail);
}