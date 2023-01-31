using RxFramework.WPF.ExtensionMethodes;
using RxFramework.WPF.Theming;
using System;
using System.Windows.Media;

namespace RxFramework.WPF.Themes;
public class Dark : Theme
{
    public override string Name => "FluentDark";
    public override ThemeType ThemeType => ThemeType.Dark;   

    public override Color GetColor(ColorKeys themeColor)
    {
        return themeColor switch
        {
            ColorKeys.Unknown => throw new NotImplementedException(),
            ColorKeys.AccentSystem => AccentColor,
            ColorKeys.AccentPrimary => AccentColor.AddValues(hue: -6, luminance: .23f),
            ColorKeys.AccentSecondary => AccentColor.AddValues(saturation: -.24d, luminance: .30d),
            ColorKeys.AcccentTeritary => AccentColor.AddValues(saturation: -.36d, luminance: .45d),
            ColorKeys.ApplicationBackground => 0xFF202020.ToColor(),
            ColorKeys.KeyboardFocusBorder => 0x87FFFFFF.ToColor(),
            ColorKeys.FontPrimary => 0xFFFFFFFF.ToColor(),
            ColorKeys.FontSecondary => 0xC5FFFFFF.ToColor(),
            ColorKeys.FontTertiary => 0x87FFFFFF.ToColor(),
            ColorKeys.FontDisabled => 0x5DFFFFFF.ToColor(),
            ColorKeys.FontPlaceholder => 0x87FFFFFF.ToColor(),
            ColorKeys.FontInverse => 0xE4000000.ToColor(),
            ColorKeys.SolidBackgroundPrimary => 0xFF202020.ToColor(),
            ColorKeys.SolidBackgroundSecondary => 0xFF1C1C1C.ToColor(),
            ColorKeys.SolidBackgroundTertiary => 0xFF282828.ToColor(),
            ColorKeys.SolidBackgroundQertiary => 0xFF2C2C2C.ToColor(),
            ColorKeys.SolidBackgroundTransparent => 0x00202020.ToColor(),
            ColorKeys.SolidBackgroundBaseAlt => 0xFFA0A0A0.ToColor(),
            ColorKeys.SolidBackgroundInverted => 0xFFFAFAFA.ToColor(),
            ColorKeys.FontAccentDisabled => AccentColor.ToHSL().L > 0.65d ? 0x77000000.ToColor() : 0xFFFFFFFF.ToColor(),
            ColorKeys.FontAccentSelectedText => AccentColor.ToHSL().L > 0.65d ? 0x00000000.ToColor() : 0xFFFFFFFF.ToColor(),
            ColorKeys.FontAccentPrimary => AccentColor.ToHSL().L > .65d ? 0xFF000000.ToColor() : 0xFFFFFFFF.ToColor(),
            ColorKeys.FontAccentSecondary => AccentColor.ToHSL().L > .65d ? 0x80000000.ToColor() : 0xFFFFFFFF.ToColor(),
            ColorKeys.Success => 0xFF6CCB5F.ToColor(),
            ColorKeys.Caution => 0xFFFCE100.ToColor(),
            ColorKeys.Critical => 0xFF99A4.ToColor(),
            ColorKeys.Neutral => 0x8BFFFFFF.ToColor(),
            ColorKeys.SuccessBackground => 0xFF393D1B.ToColor(),
            ColorKeys.CautionBackground => 0xFF433519.ToColor(),
            ColorKeys.CriticalBackground => 0xFF442726.ToColor(),
            ColorKeys.NeutralBackground => 0x08FFFFFF.ToColor(),
            ColorKeys.BackgroundPrimary => 0xFF2D2D2D.ToColor(),
            ColorKeys.BackgroundSecondary => 0x15FFFFFF.ToColor(),
            ColorKeys.BackgroundTertiary => 0x08FFFFFF.ToColor(),
            ColorKeys.BackgroundDisabled => 0x0BFFFFFF.ToColor(),
            ColorKeys.BackgroundTransparent => 0x00FFFFFF.ToColor(),
            ColorKeys.BackgroundInputActive => 0xB31E1E1E.ToColor(),
            ColorKeys.StrokePrimary => 0x12FFFFFF.ToColor(),
            ColorKeys.StrokeSecondary => 0x18FFFFFF.ToColor(),
            ColorKeys.StrokeTertiary => 0x5FFFFFF.ToColor(),
            ColorKeys.StrokeDisabled => 0x5DFFFFFF.ToColor(),
            ColorKeys.StrokeAccentPrimary => 0x14FFFFFF.ToColor(),
            ColorKeys.StrokeAccentSecondary => 0x23000000.ToColor(),
            ColorKeys.StrokeAccentTeritary => 0x37000000.ToColor(),
            ColorKeys.StrokeAccentDisabled => 0x33000000.ToColor(),
            _ => throw new ArgumentOutOfRangeException($"Color {themeColor} not defined"),
        }; ;
    }

    public override Font GetFont(FontKeys themeFont)
    {
        return themeFont switch
        {
            FontKeys.Unknown => throw new NotImplementedException(),
            FontKeys.Caption => Fonts.Caption,
            FontKeys.Body => Fonts.Body,
            FontKeys.BodyStrong => Fonts.BodyStrong,
            FontKeys.BodyLarge => Fonts.BodyLarge,
            FontKeys.Subtitle => Fonts.Subtitle,
            FontKeys.Title => Fonts.Title,
            FontKeys.TitleLarge => Fonts.TitleLarge,
            FontKeys.Display => Fonts.Display,
            _ => throw new ArgumentOutOfRangeException($"Font {themeFont} not defined"),
        };
    }
}
