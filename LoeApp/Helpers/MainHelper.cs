using System;
using System.Windows.Media;

namespace LoeApp.Helpers;

public static class MainHelper
{
    public static SolidColorBrush CreateColorBrushRgb(byte r, byte g, byte b)
    {
        return new SolidColorBrush(Color.FromRgb(r, g, b));
    }

    public static string FormatTimeSpan(TimeSpan time)
    {
        return $"{time.Hours:00}:{time.Minutes:00}";
    }

    public static int GetDayOfWeekNumber(DayOfWeek dayOfWeek)
    {
        switch (dayOfWeek)
        {

            case DayOfWeek.Monday:
                return 0;
            case DayOfWeek.Tuesday:
                return 1;
            case DayOfWeek.Wednesday:
                return 2;
            case DayOfWeek.Thursday:
                return 3;
            case DayOfWeek.Friday:
                return 4;
            case DayOfWeek.Saturday:
                return 5;
            case DayOfWeek.Sunday:
                return 6;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}