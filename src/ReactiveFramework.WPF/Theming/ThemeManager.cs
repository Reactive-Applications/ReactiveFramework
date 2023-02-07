using ReactiveFramework.WPF.Themes;
using System.Windows;
using System.Windows.Media;

namespace ReactiveFramework.WPF.Theming;
internal class ThemeManager : IThemeManager
{
    private static ResourceDictionary AppResources => Application.Current.Resources;

    private Type? _defaultTheme;

    public ThemeManager(IThemeCollection themes)
    {
        Themes = themes;
        themes.TryAdd<Dark>();
        themes.TryAdd<Light>();
    }

    public bool UseSystemDefault { get; set; } = true;

    public Theme ActiveTheme { get; private set; }

    public IThemeCollection Themes { get; }

    public void ChangeTheme(Theme theme)
    {
        if (ActiveTheme != null)
        {
            AppResources.MergedDictionaries.Remove(ActiveTheme);
        }
        ActiveTheme = theme;
        AppResources.MergedDictionaries.Add(theme);
    }

    public void ChangeTheme(string name)
    {
        var theme = Themes.GetTheme(name);
        ChangeTheme(theme);
    }

    public void SetDefaultTheme<T>() where T : Theme, new()
    {
        Themes.TryAdd<T>();
        _defaultTheme = typeof(T);
    }

    private Theme GetDefaultTheme()
    {

        if (_defaultTheme is not null)
        {
            return Themes.GetTheme(_defaultTheme);
        }

        if (ThemeHelper.AppsUseLightTheme() || !UseSystemDefault)
        {
            return Themes.GetTheme(ThemeType.Light);
        }

        return Themes.GetTheme(ThemeType.Dark);

    }

    public void Initialize()
    {
        ActiveTheme = GetDefaultTheme();
        var accentColor = ThemeHelper.GetWindowsAccentColor();
        if (accentColor == null)
        {
            Color.FromRgb(13, 17, 23);
        }
        foreach (var theme in Themes)
        {
            theme.UpdateAccentColor((Color)accentColor!);
        }
        ChangeTheme(ActiveTheme);
    }
}
