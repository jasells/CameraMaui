using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;

namespace Camera.MAUI.Ex.Services;

public interface IVideoRequirementsService : INotifyPropertyChanged, INotifyPropertyChanging
{
    long MinimumRequiredStorageMb { get; set; }
    IAsyncRelayCommand InsufficientSpaceCommand { get; set; }
    IAsyncRelayCommand MissingPermissionsCommand { get; set; }

    Task<bool> CheckAllPermissionsAsync();
    Task<(bool hasEnoughSpace, bool hasPermissions)> CheckRequirementsAsync();
    Task<bool> CheckStorageSpaceAsync();
    Task<bool> OpenAppSettingsAsync();
}