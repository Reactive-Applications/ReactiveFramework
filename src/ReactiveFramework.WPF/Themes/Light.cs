using ReactiveFramework.WPF;
using ReactiveFramework.WPF.ExtensionMethodes;
using ReactiveFramework.WPF.Theming;
using System;
using System.Windows.Media;

namespace ReactiveFramework.WPF.Themes;
public class Light : Theme
{
    public override string Name => "FluentLight";
    public override ThemeType ThemeType => ThemeType.Light;

    public override Color GetColor(ColorKeys themeColor)
    {
        return themeColor switch
        {
            ColorKeys.Unknown => throw new NotImplementedException(),
            ColorKeys.AccentSystem => AccentColor,
            ColorKeys.AccentPrimary => AccentColor.AddValues(luminance: -5d),
            ColorKeys.AccentSecondary => AccentColor.AddValues(luminance: -10d),
            ColorKeys.AcccentTeritary => AccentColor.AddValues(luminance: -15d),
            ColorKeys.ApplicationBackground => 0xFFFAFAFA.ToColor(),
            ColorKeys.KeyboardFocusBorder => 0xBE000000.ToColor(),
            ColorKeys.FontPrimary => 0xE4000000.ToColor(),
            ColorKeys.FontSecondary => 0x9E000000.ToColor(),
            ColorKeys.FontTertiary => 0x72000000.ToColor(),
            ColorKeys.FontDisabled => 0x5C000000.ToColor(),
            ColorKeys.FontPlaceholder => 0x9E000000.ToColor(),
            ColorKeys.FontInverse => 0xFFFFFFFF.ToColor(),
            ColorKeys.SolidBackgroundPrimary => 0xFFF3F3F3.ToColor(),
            ColorKeys.SolidBackgroundSecondary => 0xFFEEEEEE.ToColor(),
            ColorKeys.SolidBackgroundTertiary => 0xFFF9F9F9.ToColor(),
            ColorKeys.SolidBackgroundQertiary => 0xFFFFFFFF.ToColor(),
            ColorKeys.SolidBackgroundTransparent => 0x00F3F3F3.ToColor(),
            ColorKeys.SolidBackgroundBaseAlt => 0xFFDADADA.ToColor(),
            ColorKeys.SolidBackgroundInverted => 0xFF202020.ToColor(),
            ColorKeys.FontAccentDisabled => AccentColor.ToHSL().L > 65d ? 0x77000000.ToColor() : 0xFFFFFFF.ToColor(),
            ColorKeys.FontAccentSelectedText => AccentColor.ToHSL().L > 65d ? 0x00000000.ToColor() : 0xFFFFFFF.ToColor(),
            ColorKeys.FontAccentPrimary => AccentColor.ToHSL().L > 65d ? 0xFF000000.ToColor() : 0xFFFFFFF.ToColor(),
            ColorKeys.FontAccentSecondary => AccentColor.ToHSL().L > 65d ? 0x80000000.ToColor() : 0xFFFFFFF.ToColor(),
            ColorKeys.Success => 0xFF6CCB5F.ToColor(),
            ColorKeys.Caution => 0xFFFCE100.ToColor(),
            ColorKeys.Critical => 0xFF99A4.ToColor(),
            ColorKeys.Neutral => 0x8BFFFFFF.ToColor(),
            ColorKeys.SuccessBackground => 0xFF393D1B.ToColor(),
            ColorKeys.CautionBackground => 0xFF433519.ToColor(),
            ColorKeys.CriticalBackground => 0xFF442726.ToColor(),
            ColorKeys.NeutralBackground => 0x08FFFFFF.ToColor(),
            ColorKeys.BackgroundPrimary => 0xB3FFFFFF.ToColor(),
            ColorKeys.BackgroundSecondary => 0x80F9F9F9.ToColor(),
            ColorKeys.BackgroundTertiary => 0x4DF9F9F9.ToColor(),
            ColorKeys.BackgroundDisabled => 0x4DF9F9F9.ToColor(),
            ColorKeys.BackgroundTransparent => 0x00FFFFFF.ToColor(),
            ColorKeys.BackgroundInputActive => 0xFFFFFFFF.ToColor(),
            ColorKeys.StrokePrimary => 0x0F000000.ToColor(),
            ColorKeys.StrokeSecondary => 0x29000000.ToColor(),
            ColorKeys.StrokeTertiary => 0x9E000000.ToColor(),
            ColorKeys.StrokeDisabled => 0x5C000000.ToColor(),
            ColorKeys.StrokeAccentPrimary => 0x14FFFFFF.ToColor(),
            ColorKeys.StrokeAccentSecondary => 0x66000000.ToColor(),
            ColorKeys.StrokeAccentTeritary => 0x37000000.ToColor(),
            ColorKeys.StrokeAccentDisabled => 0xF0000000.ToColor(),
            _ => throw new ArgumentOutOfRangeException($"Color {themeColor} not defined"),
        };
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
