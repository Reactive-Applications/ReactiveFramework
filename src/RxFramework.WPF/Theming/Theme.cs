using RxFramework.WPF.ExtensionMethodes;
using System.Windows;
using System.Windows.Media;

namespace RxFramework.WPF.Theming;
public abstract class Theme : ResourceDictionary
{
    public abstract string Name { get; }

    public abstract ThemeType ThemeType { get; }

    public abstract Color GetColor(ColorKeys themeColor);

    public virtual Brush GetBrush(BrushKeys brushKey)
    {
        return brushKey switch
        {
            BrushKeys.ElevationBorder => new LinearGradientBrush()
            {
                MappingMode = BrushMappingMode.Absolute,
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 3),
                GradientStops = new GradientStopCollection
                {
                    new GradientStop()
                    {
                        Offset = 0.33,
                        Color = GetColor(ColorKeys.StrokeSecondary)
                    },
                    new GradientStop()
                    {
                        Offset = 1.0,
                        Color = GetColor(ColorKeys.StrokePrimary)
                    }
                }
            },
            BrushKeys.CircleElevationBorder => new LinearGradientBrush()
            {
                MappingMode = BrushMappingMode.RelativeToBoundingBox,
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 1),
                GradientStops = new GradientStopCollection
                {
                    new GradientStop()
                    {
                        Offset=0.7,
                        Color=GetColor(ColorKeys.StrokeSecondary)
                    },
                    new GradientStop()
                    {
                        Offset = 0.5,
                        Color=GetColor(ColorKeys.StrokePrimary)
                    }
                }
            },
            BrushKeys.AccentElevationBorder => new LinearGradientBrush()
            {
                MappingMode = BrushMappingMode.Absolute,
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 3),
                RelativeTransform = new ScaleTransform()
                {
                    CenterY = 0.5,
                    ScaleY = -1
                },
                GradientStops = new GradientStopCollection
                {
                    new GradientStop()
                    {
                        Offset=0.7,
                        Color=GetColor(ColorKeys.StrokeAccentSecondary)
                    },
                    new GradientStop()
                    {
                        Offset = 0.5,
                        Color=GetColor(ColorKeys.StrokeAccentPrimary)
                    }
                }
            },
            _ => new SolidColorBrush(GetColor(Enum.Parse<ColorKeys>(brushKey.ToString())))
        };
    }

    public abstract Font GetFont(FontKeys font);

    protected Color AccentColor { get; private set; } = 0xFF3379d9.ToColor();

    public void UpdateAccentColor(Color accentColor)
    {
        AccentColor = accentColor;
        PopulateColors();
        PopulateBrushes();
    }

    public Theme()
    {
        PopulateColors();
        PopulateFonts();
        PopulateBrushes();
    }

    private void PopulateBrushes()
    {
        foreach (var brushKey in Enum.GetValues<BrushKeys>())
        {
            if (brushKey == BrushKeys.Unknown)
            {
                continue;
            }
            var brush = GetBrush(brushKey);

            this[brushKey] = brush;
        }
    }

    private void PopulateFonts()
    {
        foreach (var fontKey in Enum.GetValues<FontKeys>())
        {
            if (fontKey == FontKeys.Unknown)
            {
                continue;
            }
            var font = GetFont(fontKey);
            this[font] = font;
        }
    }

    private void PopulateColors()
    {
        foreach (var colorKey in Enum.GetValues<ColorKeys>())
        {
            if (colorKey == ColorKeys.Unknown)
            {
                continue;
            }

            var color = GetColor(colorKey);
            this[colorKey] = color;
        }
    }
}
