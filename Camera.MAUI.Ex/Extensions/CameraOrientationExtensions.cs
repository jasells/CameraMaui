using Camera.MAUI.Ex.Models;

namespace Camera.MAUI.Ex.Extensions;

public static class CameraOrientationExtensions
{
    public static CameraOrientation ToCameraOrientation(this string name)
    {
        switch (name) 
        {
            case "Portrait": return CameraOrientation.Portrait;
            case "UpsideDown": return CameraOrientation.UpsideDown;
            case "LandscapeLeft": return CameraOrientation.LandscapeLeft;
            case "LandscapeRight": return CameraOrientation.LandscapeRight;
            default: return CameraOrientation.Unknown;
        }
    }
}