using Microsoft.Maui.Controls;
using System.Diagnostics;
using Video_Demo.ViewModel;
using Camera.MAUI.Ex.Services;

namespace Video_Demo.Views;

public partial class PermissionCheckPage : ContentPage
{
    private readonly PermissionCheckViewModel _viewModel;

    public PermissionCheckPage()
    {
        InitializeComponent();
        var videoRequirementsService = new VideoRequirementsService();
        _viewModel = new PermissionCheckViewModel(videoRequirementsService);
        BindingContext = _viewModel;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        _viewModel.ResetPermissions();
    }
} 