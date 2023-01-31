using Microsoft.Win32;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace RxFramework.WPF.Theming;

internal static class ThemeHelper
{
    private static readonly Color baseGrayColor = Color.FromRgb(217, 217, 217);

    public static bool IsHighContrastEnabled()
    {
        return SystemParameters.HighContrast;
    }

    public static bool AppsUseLightTheme()
    {
        try
        {
            var registryValue = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", null);

            if (registryValue is null)
            {
                return true;
            }

            return Convert.ToBoolean(registryValue);
        }
        catch (Exception exception)
        {
            Trace.TraceError(exception.ToString());
        }

        return true;
    }

    // Titlebars and window borders = HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\DWM\ColorPrevalence = 0 (no), 1 = yes
    public static bool ShowAccentColorOnTitleBarsAndWindowBorders()
    {
        try
        {
            var registryValue = Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\DWM", "ColorPrevalence", null);

            if (registryValue is null)
            {
                return true;
            }

            return Convert.ToBoolean(registryValue);
        }
        catch (Exception exception)
        {
            Trace.TraceError(exception.ToString());
        }

        return true;
    }


    public static Color? GetWindowsAccentColor()
    {
        return GetWindowsAccentColorFromAccentPalette()
            ?? GetWindowsColorizationColor();
    }

    public static Color? GetWindowsAccentColorFromAccentPalette()
    {
        var accentPaletteRegistryValue = Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Accent", "AccentPalette", null);

        if (accentPaletteRegistryValue is null)
        {
            return null;
        }

        try
        {
            var bin = (byte[])accentPaletteRegistryValue;

            return Color.FromRgb(bin[0x0C], bin[0x0D], bin[0x0E]);
        }
        catch (Exception exception)
        {
            Trace.TraceError(exception.ToString());
        }

        return null;
    }

    public static Color? GetWindowsColorizationColor()
    {
        var colorizationColorRegistryValue = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\DWM", "ColorizationColor", null);

        if (colorizationColorRegistryValue is null)
        {
            return null;
        }

        try
        {
            var colorizationColorTypedRegistryValue = (uint)(int)colorizationColorRegistryValue;

            // Convert colorization color to Color ignoring alpha channel.
            var colorizationColor = Color.FromRgb((byte)(colorizationColorTypedRegistryValue >> 16),
                                                  (byte)(colorizationColorTypedRegistryValue >> 8),
                                                  (byte)colorizationColorTypedRegistryValue);

            return colorizationColor;
        }
        catch (Exception exception)
        {
            Trace.TraceError(exception.ToString());
        }

        return null;
    }

    public static Color? GetBlendedWindowsAccentColor()
    {
        var colorizationColor = GetWindowsAccentColor();

        if (colorizationColor is null)
        {
            return null;
        }

        return GetBlendedColor(colorizationColor.Value);
    }

    // Thanks @https://stackoverflow.com/users/3137337/emoacht for providing the correct code on how to use ColorizationColorBalance in https://stackoverflow.com/questions/24555827/how-to-get-title-bar-color-of-wpf-window-in-windows-8-1/24600956
    public static Color? GetBlendedColor(Color color)
    {
        var colorizationColorBalanceRegistryValue = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\DWM", "ColorizationColorBalance", null);

        var colorizationColorBalance = 0D;

        if (colorizationColorBalanceRegistryValue is not null)
        {
            colorizationColorBalance = (int)colorizationColorBalanceRegistryValue;
        }

        return GetBlendedColor(color, baseGrayColor, 100 - colorizationColorBalance);
    }

    public static Color GetBlendedColor(Color color, double colorBalance)
    {
        return GetBlendedColor(color, baseGrayColor, 100 - colorBalance);
    }

    public static Color GetBlendedColor(Color color, Color baseColor, double colorBalance)
    {
        // Blend the two colors using colorization color balance parameter.
        return BlendColor(color, baseColor, 100 - colorBalance);
    }

    private static Color BlendColor(Color color1, Color color2, double color2Percentage)
    {
        if (color2Percentage is < 0 or > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(color2Percentage));
        }

        return Color.FromRgb(BlendColorChannel(color1.R, color2.R, color2Percentage),
                             BlendColorChannel(color1.G, color2.G, color2Percentage),
                             BlendColorChannel(color1.B, color2.B, color2Percentage));
    }

    private static byte BlendColorChannel(double channel1, double channel2, double channel2Percentage)
    {
        // ReSharper disable once ArrangeRedundantParentheses
        var buff = channel1 + ((channel2 - channel1) * channel2Percentage / 100D);
        return Math.Min((byte)Math.Round(buff), (byte)255);
    }
}
