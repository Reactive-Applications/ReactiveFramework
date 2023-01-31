using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RxFramework.WPF.Theming;
public static class DesignTimeThemeManger
{
    private static IThemeManager? ThemeManager;

    public static object GetTheme(DependencyObject obj)
    {
        return obj.GetValue(ThemeProperty);
    }

    public static void SetTheme(DependencyObject obj, object value)
    {
        obj.SetValue(ThemeProperty, value);
    }

    // Using a DependencyProperty as the backing store for Theme.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ThemeProperty =
        DependencyProperty.RegisterAttached("Theme", typeof(object), typeof(DesignTimeThemeManger), new PropertyMetadata(default, ThemeChangedCallBack));

    private static void ThemeChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (!DesignerProperties.GetIsInDesignMode(d))
        {
            return;
        }
        var coll = new ThemeCollection();
        ThemeManager ??= new ThemeManager(coll);

        if(e.NewValue is Theme theme)
        {
            ThemeManager.ChangeTheme(theme);
            return;
        }

        if(e.NewValue is string themeName)
        {
            ThemeManager.ChangeTheme(themeName);
            return;
        }

        throw new NotSupportedException();
    }
}
