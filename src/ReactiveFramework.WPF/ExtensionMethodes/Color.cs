using ReactiveFramework.WPF;
using System.Windows.Documents;
using System.Windows.Media;

namespace ReactiveFramework.WPF.ExtensionMethodes;

public static class ColorExtensions
{

    /// <summary>
    /// Darkens the color 
    /// </summary>
    /// <param name="color"></param>
    /// <param name="value">Range from 0 to 1</param>
    /// <returns></returns>
    public static Color Darken(this Color color, double value)
    {
        return color
                .ToHSL()
                .Darken(value)
                .ToColor();
    }

    /// <summary>
    /// Lightens the color
    /// </summary>
    /// <param name="color"></param>
    /// <param name="value">Range from 0 to 1</param>
    /// <returns></returns>
    public static Color Lighten(this Color color, double value)
    {
        return color
                .ToHSL()
                .Lighten(value)
                .ToColor();
    }

    public static SolidColorBrush Darken(this SolidColorBrush brush, uint percentage)
    {
        var col = brush.Color;
        return new SolidColorBrush(col.Darken(percentage));
    }

    public static SolidColorBrush Lighten(this SolidColorBrush brush, uint percentage)
    {
        var col = brush.Color;
        return new SolidColorBrush(col.Lighten(percentage));
    }

    public static SolidColorBrush ToBrush(this Color color)
    {
        return new SolidColorBrush(color);
    }

    public static Color ToColor(this uint argb)
    {
        return Color.FromArgb((byte)((argb & -16777216) >> 0x18),
                              (byte)((argb & 0xff0000) >> 0x10),
                              (byte)((argb & 0xff00) >> 8),
                              (byte)(argb & 0xff));
    }

    public static Color ToColor(this int argb)
    {
        var bytes = BitConverter.GetBytes(argb);

        return Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
    }

    public static HSLColor ToHSL(this Color color)
    {
        var r = color.R / 255d;
        var g = color.G / 255d;
        var b = color.B / 255d;
        var max = Math.Max(r, Math.Max(g, b));
        var min = Math.Min(r, Math.Min(g, b));
        var delta = max - min;

        double h = 0;
        double s = 0;
        double l = (max + min) / 2d;

        if (delta != 0)
        {
            if (l < 0.5)
            {
                s = delta / (max + min);
            }
            else
            {
                s = delta / (2 - max - min);
            }

            if (r == max)
            {
                h = (g - b) / delta;
            }
            else if (g == max)
            {
                h = 2 + (b - r) / delta;
            }
            else if (b == max)
            {
                h = 4 + (r - g) / delta;
            }
        }

        return new HSLColor()
        {
            H = h * 60,
            S = s,
            L = l
        };
    }

    public static HSLColor AddValues(this HSLColor hsl, double hue = 0, double saturation = 0, double luminance = 0)
    {
        hsl.H = (hsl.H + hue).Clip(0, 360);
        hsl.S = (hsl.S + saturation).Clip(0, 1);
        hsl.L = (hsl.L + luminance).Clip(0, 1);

        return hsl;
    }

    public static Color AddValues(this Color color, double hue = 0, double saturation = 0, double luminance = 0)
    {
        return color.ToHSL()
            .AddValues(hue, saturation, luminance)
            .ToColor();
    }

    public static Color Invert(this Color color)
    {
        return Color.FromRgb((byte)(255 - color.R), (byte)(255 - color.G), (byte)(255 - color.B));
    }

}
