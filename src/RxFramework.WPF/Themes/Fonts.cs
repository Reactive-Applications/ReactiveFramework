using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace RxFramework.WPF.Themes;
public static class Fonts
{

    public static Font Body = new()
    {
        FontFamily = new FontFamily("Segoe UI"),
        Size = 14,
        Weight = FontWeights.Regular,
        Strech = FontStretches.Normal,
        Style = FontStyles.Normal
    };

    public static Font BodyStrong = new(Body)
    {
        Weight = FontWeights.SemiBold
    };

    public static Font BodyLarge = new(Body)
    {
        Size = 18
    };

    public static Font Caption = new(Body)
    {
        Size = 12
    };

    public static Font Subtitle = new(Body)
    {
        Size = 20,
        Weight = FontWeights.SemiBold,
    };

    public static Font Title = new(Body)
    {
        Size = 28,
        Weight = FontWeights.SemiBold,
    };

    public static Font TitleLarge = new(Body)
    {
        Size = 40,
        Weight = FontWeights.SemiBold,
    };

    public static Font Display = new(Body)
    {
        Size = 68,
        Weight = FontWeights.SemiBold,
    };
}
