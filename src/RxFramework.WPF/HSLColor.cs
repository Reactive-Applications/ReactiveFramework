using System.Windows.Media;

namespace RxFramework.WPF;
public struct HSLColor
{
    public double H { get; set; }
    public double S { get; set; }
    public double L { get; set; }

    public HSLColor()
    {
    }

    public HSLColor(double h, double s, double l)
    {
        H = h;
        S = s;
        L = l;
    }

    public Color ToColor()
    {
        byte r;
        byte g;
        byte b;

        if (S == 0)
        {
            r = g = b = (byte)(L * 255);
        }
        else
        {
            double v1, v2;
            double hue = H / 360;

            v2 = (L < 0.5) ? (L * (1 + S)) : (L + S - L * S);
            v1 = 2 * L - v2;

            r = (byte)(255 * HueToRGB(v1, v2, hue + 1.0f / 3));
            g = (byte)(255 * HueToRGB(v1, v2, hue));
            b = (byte)(255 * HueToRGB(v1, v2, hue - 1.0f / 3));
        }

        return Color.FromRgb(r, g, b);
    }

    public HSLColor Darken(double value)
    {
        if (value > 1 || value < 0)
        {
            throw new ArgumentException("value must be between 0 and 1");
        }

        return new HSLColor
        {
            H = H,
            S = S,
            L = L * (1 - value)
        };
    }

    public HSLColor Lighten(double value)
    {
        if (value < 0)
        {
            throw new ArgumentException("value must be greater then 0");
        }

        var l = (1 - L) * value + L;
        if (l > 1)
        {
            l = 1;
        }

        return new HSLColor
        {
            H = H,
            S = S,
            L = l
        };
    }

    private double HueToRGB(double v1, double v2, double vH)
    {
        if (vH < 0)
        {
            vH += 1;
        }

        if (vH > 1)
        {
            vH -= 1;
        }

        if (6 * vH < 1)
        {
            return v1 + (v2 - v1) * 6 * vH;
        }

        if (2 * vH < 1)
        {
            return v2;
        }

        if (3 * vH < 2)
        {
            return v1 + (v2 - v1) * (2.0f / 3 - vH) * 6;
        }

        return v1;
    }

    public HSLColor Invert()
    {
        return new HSLColor()
        {
            H = (H + 180) % 360,
            S = S,
            L = L,
        };
    }
}
