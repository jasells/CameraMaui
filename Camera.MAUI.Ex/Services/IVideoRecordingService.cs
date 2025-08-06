using System.ComponentModel;

namespace Camera.MAUI.Ex.Services;

public interface IVideoRecordingService: INotifyPropertyChanged, INotifyPropertyChanging
{
    /// <summary>
    /// The file path where the recorded video will be saved.
    /// </summary>
    string FilePath { get; set; }
    /// <summary>
    /// True if VideoRecordState is Recording, false otherwise.
    /// </summary>
    bool RecordVideo { get; set; }
    /// <summary>
    /// Seconds remaining for the allowed time for video recording.
    /// </summary>
    double SecondsRemaining { get; set; }
    /// <summary>
    /// Max seconds allowed for video recording.
    /// </summary>
    double MaxSeconds { get; set; }
    /// <summary>
    /// The threshold time in seconds for the alert to be shown when the recording is about to end.
    /// </summary>
    int AlertTimeRemaining { get; set; }
    /// <summary>
    /// True if the user can start recording a video, false otherwise.
    /// </summary>
    bool RecordButtonEnabled { get; set; }
    /// <summary>
    /// The current state of the video recording process.
    /// </summary>
    Models.VideoRecordState CurrentState { get; set; }
    /// <summary>
    /// Recording complete command that is executed when the recording process is finished.
    /// </summary>
    CommunityToolkit.Mvvm.Input.IAsyncRelayCommand RecordingComplete { get; set; }
    /// <summary>
    /// Start recording command that initiates the video recording process.
    /// </summary>
    CommunityToolkit.Mvvm.Input.IAsyncRelayCommand<string> StartRecordingCommand { get; set; }
    /// <summary>
    /// Toggles the video recording state between recording and not recording.
    /// </summary>
    CommunityToolkit.Mvvm.Input.IAsyncRelayCommand ToggleVideoCommand { get; }
    /// <summary>
    /// Stops the video recording process if it is currently in the recording state.
    /// Optionally triggers the RecordingComplete command upon completion.
    /// </summary>
    /// <param name="callRecordingCompleteCmd">
    /// A boolean value indicating whether to execute the RecordingComplete command after stopping the recording.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation of stopping the recording.
    /// </returns>
    Task StopRecordingInternal(bool callRecordingCompleteCmd);
    /// <summary>
    /// The dispatcher associated with the current RecordVideoView (onscreen)
    /// </summary>
    IDispatcher Dispatcher { get; set; }
    /// <summary>
    /// The message displayed on tool tips when the user has ongoing recording.
    /// </summary>
    string OngoingRecordingMessage { get; set; }
    /// <summary>
    /// The message displayed on tool tips when the user can start recording a video.
    /// </summary>
    string StartRecordingMessage { get; set; }
    /// <summary>
    /// The text color of the message displayed on tool tips for both start recording and ongoing recording messages.
    /// </summary>
    Color RecordingMessageTextColor { get; set; }
    /// <summary>
    /// The background color of the message displayed on tool tips for both start recording and ongoing recording messages.
    /// </summary>
    Color RecordingMessageBackgroundColor { get; set; }
    /// <summary>
    /// The font size of the message displayed on tool tips for both start recording and ongoing recording messages.
    /// </summary>
    double RecordingMessageFontSize { get; set; }
    /// <summary>
    /// The padding of the message displayed on tool tips for both start recording and ongoing recording messages.
    /// </summary>
    Thickness RecordingMessagePadding { get; set; }
    /// <summary>
    /// The font size of the countdown timer text
    /// </summary>
    double TimerFontSize { get; set; }
    /// <summary>
    /// The background color of the countdown timer view
    /// </summary>
    Color HeaderBackgroundColor { get; set; }
    /// <summary>
    /// The background color of the record view
    /// </summary>
    Color RecordViewBackgroundColor { get; set; }
    /// <summary>
    /// The color of the progress bar in the countdown timer view
    /// </summary>
    Color ProgressColor { get; set; }
    /// <summary>
    /// The height of the progress bar in the countdown timer view
    /// </summary>
    double ProgressHeight { get; set; }
    /// <summary>
    /// Default text color of the countdown timer when it is not ending
    /// </summary>
    Color TimerDefaultTextColor { get; set; }
    /// <summary>
    /// Default background color of the countdown timer when it is not ending
    /// </summary>
    Color TimerDefaultBackgroundColor { get; set; }
    /// <summary>
    /// Text color of the countdown timer when it is ending
    /// </summary>
    Color TimerEndingTextColor { get; set; }
    /// <summary>
    /// Background color of the countdown timer when it is ending
    /// </summary>
    Color TimerEndingBackgroundColor { get; set; }
    /// <summary>
    /// Padding of the countdown timer label
    /// </summary>
    Thickness TimerLabelPadding { get; set; }
    /// <summary>
    /// The actual formatted value displayed on the countdown timer text
    /// </summary>
    string SecondsRemainingFormatted { get; }
    /// <summary>
    /// The progress of the countdown timer that will be used on progress bar, represented as a value between 0.0 and 1.0.
    /// </summary>
    double Progress { get; }
    /// <summary>
    /// The timer color of the countdown timer text, value may vary based on the state of the timer
    /// </summary>
    Color TimerTextColor { get; }
    /// <summary>
    /// The background color of the countdown timer text, value may vary based on the state of the timer
    /// </summary>
    Color TimerBackgroundColor { get; }
    /// <summary>
    /// Padding of the countdown timer container
    /// </summary>
    Thickness CountdownTimerViewPadding { get; set; }
    /// <summary>
    /// The space between the countdown timer text and the progress bar
    /// </summary>
    double CountdownTimerViewSpacing { get; set; }
    /// <summary>
    /// The size of the Record button
    /// </summary>
    double RecordButtonSize { get; set; }
    /// <summary>
    /// Outline color for Record button
    /// </summary>
    Color RecordButtonOutlineColor { get; set; }
    /// <summary>
    /// Outline width for Record button
    /// </summary>
    double RecordButtonOutlineWidth { get; set; }
    /// <summary>
    /// Play icon size for Record button - calculated 80% of RecordButtonSize
    /// </summary>
    double PlayIconSize { get; }
    /// <summary>
    /// Stop icon size for Record button - calculated 40% of RecordButtonSize
    /// </summary>
    double StopIconSize { get; }
    /// <summary>
    /// Play icon color for Record button
    /// </summary>
    Color PlayIconColor { get; set; }
    /// <summary>
    /// Stop icon color for Record button
    /// </summary>
    Color StopIconColor { get; set; }
    /// <summary>
    /// Margin around the record button and start recording message container
    /// </summary>
    Thickness RecordButtonMargin { get; set; }
    /// <summary>
    /// Space between start recording message and record button
    /// </summary>
    double StartRecordingBottomSpacing { get; set; }
    /// <summary>
    /// Font family for the countdown timer text
    /// </summary>
    string TimerFontFamily { get; set; }
    /// <summary>
    /// The font family of the message displayed on tool tips for both start recording and ongoing recording messages.
    /// </summary>
    string RecordingMessageFontFamily { get; set; }
}