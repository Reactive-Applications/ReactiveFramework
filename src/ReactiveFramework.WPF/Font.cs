using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ReactiveFramework.WPF;
public class Font
{
    public static Font GetFont(DependencyObject obj)
    {
        return (Font)obj.GetValue(FontProperty);
    }

    public static void SetFont(DependencyObject obj, Font value)
    {
        obj.SetValue(FontProperty, value);
    }

    // Using a DependencyProperty as the backing store for Font.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty FontProperty =
        DependencyProperty.RegisterAttached("Font", typeof(Font), typeof(Control), new PropertyMetadata(null, FontChangedCallBack));

    private static void FontChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {

        if (e.NewValue is Font font)
        {

            Apply(font, d);
        }
        else if (e.NewValue is string fontName)
        {
            var fontRes = Application.Current.Resources[fontName] as Font;

            if (fontRes is null)
            {
                throw new ArgumentException("Font not found");
            }

            Apply(fontRes, d);
        }
    }

    public string? Name { get; set; }

    public FontFamily FontFamily { get; set; }

    public double Size { get; set; }

    public FontStretch Strech { get; set; }

    public FontStyle Style { get; set; }

    public FontWeight Weight { get; set; }



    public Font()
    {
        FontFamily = new FontFamily("Segoe UI");
    }

    public Font(Font baseFont)
    {
        FontFamily = baseFont.FontFamily;
        Size = baseFont.Size;
        Strech = baseFont.Strech;
        Style = baseFont.Style;
        Weight = baseFont.Weight;
    }

    public static void Apply(Font font, DependencyObject d)
    {
        d.SetValue(Control.FontFamilyProperty, font.FontFamily);
        d.SetValue(Control.FontSizeProperty, font.Size);
        d.SetValue(Control.FontStretchProperty, font.Strech);
        d.SetValue(Control.FontStyleProperty, font.Style);
        d.SetValue(Control.FontWeightProperty, font.Weight);
    }
}
