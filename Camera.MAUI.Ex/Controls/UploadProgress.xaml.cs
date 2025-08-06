namespace Camera.MAUI.Ex.Controls;

public partial class UploadProgress : ContentView
{
    public Services.IUploadProgressService UploadService
    {
        get => _upSrv;
        set
        {
            _upSrv = value;
            OnPropertyChanged();
        }
    }
    private Services.IUploadProgressService _upSrv;


    public UploadProgress(Services.IUploadProgressService uploadSrvc)
	{
		InitializeComponent();

        UploadService = uploadSrvc;
	}
}