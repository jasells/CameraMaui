using Android.App;
using Android.Content.PM;
using Android.OS;

namespace Camera.MAUI.Test;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ScreenOrientation = ScreenOrientation.FullUser, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        Java.Lang.Thread.DefaultUncaughtExceptionHandler =
            new GlobalExceptionHandler();
    }

}


/// <summary>
/// To handle java errors often thrown by threads interacting with camera and log them to logcat,
/// since they will crash the app but just get swallowed by the system and make it hard to debug.
/// </summary>
public class GlobalExceptionHandler : Java.Lang.Object, Java.Lang.Thread.IUncaughtExceptionHandler
{
    public void UncaughtException(Java.Lang.Thread thread, Java.Lang.Throwable ex)
    {
        Android.Util.Log.Error("GlobalJavaCrash", ex.ToString());
    }
}
