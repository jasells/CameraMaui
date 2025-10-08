namespace Camera.MAUI;


/// <summary>
/// Inject/bind this to viewmodels to get camera information and control the specific camera.
/// </summary>
public class CameraInfo : Internals.ObservableBase
{
    public string Name
    {
        get => _name;
        internal set => SetProperty(ref _name, value);
    }
    private string _name;

    public string DeviceId
    {
        get => _deviceId;
        internal set => SetProperty(ref _deviceId, value);
    }
    private string _deviceId;

    public CameraPosition Position
    {
        get => _position;
        internal set => SetProperty(ref _position, value);
    }
    private CameraPosition _position;

    public bool HasFlashUnit
    {
        get => _hasFlashUnit;
        internal set => SetProperty(ref _hasFlashUnit, value);
    }
    private bool _hasFlashUnit;

    public float MinZoomFactor
    {
        get => _minZoomFactor;
        internal set => SetProperty(ref _minZoomFactor, value);
    }
    private float _minZoomFactor;

    public float MaxZoomFactor
    {
        get => _maxZoomFactor;
        internal set => SetProperty(ref _maxZoomFactor, value);
    }
    private float _maxZoomFactor;

    public float HorizontalViewAngle
    {
        get => _horizontalViewAngle;
        internal set => SetProperty(ref _horizontalViewAngle, value);
    }
    private float _horizontalViewAngle;

    public float VerticalViewAngle
    {
        get => _verticalViewAngle;
        internal set => SetProperty(ref _verticalViewAngle, value);
    }
    private float _verticalViewAngle;

    /// <summary>
    /// This is used to indicate if the camera feed is mirrored, and allows the 
    /// state to track the CAMERA, not the view/control <see cref="CameraView"/>, since it 
    /// can hold different cameras, with different mirrored states.
    /// </summary>
    public bool IsMirrored
    {
        get => _isMirrored;
        internal set => SetProperty(ref _isMirrored, value);
    }
    private bool _isMirrored;

    public List<Size> AvailableResolutions
    {
        get => _availableResolutions;
        internal set => SetProperty(ref _availableResolutions, value);
    }
    private List<Size> _availableResolutions;

    public override string ToString()
    {
        return Name;
    }
}
