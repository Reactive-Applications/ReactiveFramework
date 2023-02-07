using RxFramework.WPF.FluentControls.Windowing;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RxFramework.WPF.FluentControls.Text;

public class TextBox : System.Windows.Controls.TextBox
{

    public string PlaceholderText
    {
        get { return (string)GetValue(PlaceholderTextProperty); }
        set { SetValue(PlaceholderTextProperty, value); }
    }

    // Using a DependencyProperty as the backing store for PlaceHolderText.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty PlaceholderTextProperty =
        DependencyProperty.Register("PlaceholderText", typeof(string), typeof(TextBox), new PropertyMetadata(string.Empty));



    public bool ShowPlaceholderText
    {
        get { return (bool)GetValue(ShowPlaceholderTextProperty); }
        set { SetValue(ShowPlaceholderTextProperty, value); }
    }

    // Using a DependencyProperty as the backing store for ShowPlaceholderText.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ShowPlaceholderTextProperty =
        DependencyProperty.Register("ShowPlaceholderText", typeof(bool), typeof(TextBox), new PropertyMetadata(true));




    public string Label
    {
        get { return (string)GetValue(LabelProperty); }
        set { SetValue(LabelProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Label.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register("Label", typeof(string), typeof(TextBox), new PropertyMetadata(string.Empty));


    public bool ShowLabel
    {
        get { return (bool)GetValue(ShowLabelProperty); }
        set { SetValue(ShowLabelProperty, value); }
    }

    // Using a DependencyProperty as the backing store for ShowLabel.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ShowLabelProperty =
        DependencyProperty.Register("ShowLabel", typeof(bool), typeof(TextBox), new PropertyMetadata(true));


    public Dock LabelPosition
    {
        get { return (Dock)GetValue(LabelPositionProperty); }
        set { SetValue(LabelPositionProperty, value); }
    }



    public Thickness LabelMargin
    {
        get { return (Thickness)GetValue(LabelMarginProperty); }
        set { SetValue(LabelMarginProperty, value); }
    }

    // Using a DependencyProperty as the backing store for LabelMargin.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty LabelMarginProperty =
        DependencyProperty.Register("LabelMargin", typeof(Thickness), typeof(TextBox), new PropertyMetadata(new Thickness(5)));



    public Font LabelFont
    {
        get { return (Font)GetValue(LabelFontProperty); }
        set { SetValue(LabelFontProperty, value); }
    }

    // Using a DependencyProperty as the backing store for LabelFont.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty LabelFontProperty =
        DependencyProperty.Register("LabelFont", typeof(Font), typeof(TextBox), new PropertyMetadata(default, LabelFontChangedCallBack));

    private static void LabelFontChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is Font font)
        {
            ((TextBox)d).ApplyFontToLabel(font);
        }
    }


    // Using a DependencyProperty as the backing store for LabelPosition.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty LabelPositionProperty =
        DependencyProperty.Register("LabelPosition", typeof(Dock), typeof(TextBox), new PropertyMetadata(Dock.Top));

    public Brush Indicator
    {
        get { return (Brush)GetValue(IndicatorProperty); }
        set { SetValue(IndicatorProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Indicator.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty IndicatorProperty =
        DependencyProperty.Register("Indicator", typeof(Brush), typeof(TextBox), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));




    public CornerRadius CornerRadius
    {
        get { return (CornerRadius)GetValue(CornerRadiusProperty); }
        set { SetValue(CornerRadiusProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(TextBox), new PropertyMetadata(new CornerRadius(10)));



    public Font Font
    {
        get { return (Font)GetValue(FontProperty); }
        set { SetValue(FontProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Font.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty FontProperty =
        DependencyProperty.Register("Font", typeof(Font), typeof(TextBox), new PropertyMetadata(default, FontChangedCallBack));

    private static void FontChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is Font font)
        {
            Font.Apply(font, d);
        }
    }

    static TextBox()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBox), new FrameworkPropertyMetadata(typeof(TextBox)));
    }

    private void ApplyFontToLabel(Font font)
    {
        var label = Template.FindName("Label", this) as Label;

        if (label is not null)
        {
            Font.Apply(font, label);
        }

    }
}
