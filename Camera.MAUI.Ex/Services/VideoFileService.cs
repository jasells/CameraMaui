using System;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Camera.MAUI.Ex.Services;

public class VideoFileService : NotifyPropertyChangedBase, IVideoFileService
{
    ///<inheritdoc cref="IVideoFileService"/>
    public async Task<bool> DeleteVideoFile(string filePath)
    {
        try
        {
            if (string.IsNullOrEmpty(filePath))
            {
                Debug.WriteLine("VideoFileService: File path is null or empty");
                return false;
            }

            if (!File.Exists(filePath))
            {
                Debug.WriteLine($"VideoFileService: File does not exist at path: {filePath}");
                return false;
            }

            await Task.Run(() => File.Delete(filePath));
            Debug.WriteLine($"VideoFileService: Successfully deleted video file at path: {filePath}");
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"VideoFileService: Failed to delete video file at path: {filePath}. Error: {ex.Message}");
            return false;
        }
    }
} 