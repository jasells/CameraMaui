namespace Camera.MAUI.Ex.Extensions;

public static class DateTimeHelper
{
    public static string FormatTime(long milliseconds)
    {
        var totalSeconds = milliseconds / 1000.0;
        var minutes = (int)(totalSeconds / 60);
        var seconds = (int)(totalSeconds % 60);
        
        if (minutes == 0)
        {
            seconds += 1;
        }
    
        return $"{minutes}:{seconds:D2}";
    }
}