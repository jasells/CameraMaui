using System.ComponentModel;
using System.Threading.Tasks;

namespace Camera.MAUI.Ex.Services;

public interface IVideoFileService: INotifyPropertyChanged, INotifyPropertyChanging
{
    /// <summary>
    /// Deletes a video file from the local storage
    /// </summary>
    /// <param name="filePath">Path to the video file to delete</param>
    /// <returns>True if deletion was successful, false otherwise</returns>
    Task<bool> DeleteVideoFile(string filePath);
} 