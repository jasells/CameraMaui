namespace Camera.MAUI.Ex.Services
{
    public interface IUploadProgressService :
                        System.ComponentModel.INotifyPropertyChanged,
                        System.ComponentModel.INotifyPropertyChanging
    {
        double ProgressWidthRequest { get; set; }
        GridLength ProgressColumnWidth { get; }
        double ColumnSpacing { get; set; }
        
        double Progress { get; set; }
        Color ProgressColor { get; set; }
        Color ProgressBackGroundColor { get; set; }
        double ProgressHeightRequest { get; set; }
        
        string ProgressPercent { get; set; }
        Color ProgressPercentTextColor { get; set; }
        Color ProgressPercentTextBackColor { get; set; }
        double ProgressPercentFontSize { get; set; }
        string ProgressPercentFont { get; set; }

        string UploadStatusMessage { get; set; }
        Thickness StatusLabelPadding { get; set; }
        Color UploadStatusTextColor { get; set; }
        Color UploadStatusTextBackColor { get; set; }
        double UploadStatusFontSize { get; set; }
        string UploadStatusFont { get; set; }
        
        string InProgressMessage { get; set; }
        string UploadCompleteMessage { get; set; }
    }
}