using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Debug = System.Diagnostics.Debug;

namespace Camera.MAUI.Ex.Services;


public interface IUploadAdapter: System.ComponentModel.INotifyPropertyChanged,
                                 System.ComponentModel.INotifyPropertyChanging
{
    IUploadProgressService ProgressService { get; set; }

    Task UploadVideoAsync(string filePath);
}

public abstract class UploadAdapter : NotifyPropertyChangedBase, IUploadAdapter
{

    public IUploadProgressService ProgressService
    {
        get => _progSrvc;
        set => SetProperty(ref _progSrvc, value);
    }
    private IUploadProgressService _progSrvc;

    public UploadAdapter(IUploadProgressService progressService,
                         string completeMsg = "Upload is complete!",
                         string inProgressMsg = "Your video is uploading.")
    {
        ProgressService = progressService ?? throw new ArgumentNullException(nameof(progressService));

        // there, now we can override these in the derived classes.
        ProgressService.UploadCompleteMessage = completeMsg;
        ProgressService.InProgressMessage = inProgressMsg;
    }

    /// <summary>
    /// override to provide specific implementation for uploading a video file.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public abstract Task UploadVideoAsync(string filePath);

    /// <summary>
    /// Updates the <see cref="IUploadProgressService"/> properties with the current upload progress.
    /// </summary>
    /// <param name="progress"></param>
    protected void UpdateProgress(double progress)
    {
        ProgressService.Progress = progress;

        Debug.WriteLine($"===== uploading :{progress * 100f} %");
    }
}
