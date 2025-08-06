using System.Runtime.CompilerServices;

namespace Camera.MAUI.Ex.Services;

public class UploadProgressService : NotifyPropertyChangedBase, IUploadProgressService
{
    #region progress bar properties
    /// <summary>
    /// Gets or sets the upload progress from <c>0.00</c> to <c>1.00</c>
    /// </summary>
    public double Progress
    {
        get => _progress;
        set => SetProperty(ref _progress, value);
    }
    private double _progress = 0;
    
    public string ProgressPercent
    {
        get => _progPercent;
        set => SetProperty(ref _progPercent, value);
    }
    private string _progPercent;
    
    public Color ProgressPercentTextColor
    {
        get => _percentTextColor;
        set => SetProperty(ref _percentTextColor, value);
    }
    private Color _percentTextColor = Resources.Colors.Blackbird60;

    public Color ProgressColor
    {
        get => _progColor;
        set => SetProperty(ref _progColor, value);
    }
    private Color _progColor = Resources.Colors.Leaf;
    
    public Color ProgressPercentTextBackColor
    {
        get => _progressPercentTextBackColor;
        set => SetProperty(ref _progressPercentTextBackColor, value);
    }
    private Color _progressPercentTextBackColor = Colors.Transparent;

    /// <summary>
    /// Gets or sets the font size for the upload status display. Default is <c>14.0</c>
    /// </summary>
    public double ProgressPercentFontSize
    {
        get => _progressPercentFontSize;
        set => SetProperty(ref _progressPercentFontSize, value);
    }
    private double _progressPercentFontSize = 14.0;
    
    public string ProgressPercentFont
    {
        get => _progressPercentFont;
        set => SetProperty(ref _progressPercentFont, value);
    }
    private string _progressPercentFont = Resources.FontNames.PromptRegular;

    /// <summary>
    /// sets <see cref="VisualElement.BackgroundColor"/> on <see cref="ProgressBar"/> <para/>
    /// Default is <see cref="Colors.Transparent"/> <para/>
    /// </summary>
    public Color ProgressBackGroundColor
    {
        get => _backColor;
        set => SetProperty(ref _backColor, value);
    }
    private Color _backColor = Colors.Transparent;

    /// <summary>
    /// sets the height on <see cref="ProgressBar"/>.  Default is <c>20.0</c>
    /// </summary>
    public double ProgressHeightRequest
    {
        get => _height;
        set => SetProperty(ref _height, value);
    }
    private double _height = 20.0;
    #endregion

    #region status label properties
    public Thickness StatusLabelPadding
    {
        get => _statusPadding;
        set => SetProperty(ref _statusPadding, value);
    }
    private Thickness _statusPadding = new Thickness(4, 2, 4, 2);

    /// <summary>
    /// Gets or sets the upload status message.
    /// </summary>
    public string UploadStatusMessage
    {
        get => _uploadStatusMessage;
        set => SetProperty(ref _uploadStatusMessage, value);
    }
    private string _uploadStatusMessage = "Uploading...";
    
    public Color UploadStatusTextColor
    {
        get => _textColor;
        set => SetProperty(ref _textColor, value);
    }
    private Color _textColor = Resources.Colors.Blackbird;
    
    public Color UploadStatusTextBackColor
    {
        get => _textBackColor;
        set => SetProperty(ref _textBackColor, value);
    }
    private Color _textBackColor = Colors.Transparent;

    /// <summary>
    /// Gets or sets the font size for the upload status display. Default is <c>14.0</c>
    /// </summary>
    public double UploadStatusFontSize
    {
        get => _statusFontSize;
        set => SetProperty(ref _statusFontSize, value);
    }
    private double _statusFontSize = 14.0;
    
    public string UploadStatusFont
    {
        get => _statusFont;
        set => SetProperty(ref _statusFont, value);
    }
    private string _statusFont = Resources.FontNames.PromptRegular;

    /// <summary>
    /// Default values <c>'Upload is complete!'</c>
    /// </summary>
    public string UploadCompleteMessage
    {
        get => _completeMsg;
        set => SetProperty(ref _completeMsg, value);
    }
    private string _completeMsg = "Upload is complete!";

    /// <summary>
    /// Default value <c>'Your video is uploading.'</c>
    /// </summary>
    public string InProgressMessage
    {
        get => _progMsg;
        set => SetProperty(ref _progMsg, value);
    }
    private string _progMsg = "Your video is uploading.";
    #endregion

    #region grid spacing properties
    /// <summary>
    /// Spacing between the internal grid columns, default is <c>20.0</c>
    /// </summary>
    public double ColumnSpacing
    {
        get => _columnSpacing;
        set => SetProperty(ref _columnSpacing, value);
    }
    private double _columnSpacing = 15.0;
    
    public double ProgressWidthRequest
    {
        get => _progressWidthRequest;
        set
        {
            SetProperty(ref _progressWidthRequest, value);
            OnPropertyChanged(nameof(ProgressColumnWidth));
        }
    }
    private double _progressWidthRequest = 70.0;
    
    public GridLength ProgressColumnWidth => new GridLength(ProgressWidthRequest);
    #endregion

    public UploadProgressService()
    {}

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == nameof(Progress))
        {
            UploadStatusMessage = Progress < 1.0f ? InProgressMessage : UploadCompleteMessage;

            ProgressPercent = $"({Progress * 100f:F0}%)";
        }
    }
}
