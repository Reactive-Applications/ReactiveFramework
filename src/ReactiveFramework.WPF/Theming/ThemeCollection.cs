using System.Collections;
using System.Windows;

namespace ReactiveFramework.WPF.Theming;

public class ThemeCollection : IThemeCollection
{
    private Dictionary<string, Theme> _themes = new();

    public void Add<T>() where T : Theme, new()
    {
        Add(typeof(T));
    }

    public void Add(Type themeType)
    {
        if (!themeType.IsAssignableTo(typeof(Theme)))
        {
            throw new ArgumentException($"Theme must be of type: {typeof(Theme)}");
        }

        var theme = (Theme)Activator.CreateInstance(themeType)!;
        Add(theme);
    }

    public void Add(Theme theme)
    {
        _themes.Add(theme.Name, theme);
    }

    public IEnumerator<Theme> GetEnumerator()
    {
        return _themes.Values.GetEnumerator();
    }

    public Theme GetTheme<T>()
    {
        return GetTheme(typeof(T));
    }

    public Theme GetTheme(Type themeType)
    {
        var theme = _themes.Values.FirstOrDefault(t => t.GetType() == themeType);

        if (theme == null)
        {
            throw new KeyNotFoundException("Theme not found in collection");
        }

        return theme;
    }

    public Theme GetTheme(string themeName)
    {
        return _themes[themeName];
    }

    public Theme GetTheme(ThemeType themeType)
    {
        var theme = _themes.Values.FirstOrDefault(t => t.ThemeType == themeType);
        if (theme is null)
        {
            throw new KeyNotFoundException("Theme not found in collection");
        }

        return theme;
    }

    public void Remove<T>() where T : Theme, new()
    {
        Remove(typeof(T));
    }

    public void Remove(Type themeType)
    {
        var theme = _themes.Values.FirstOrDefault(t => t.GetType() == themeType);
        if (theme is not null)
        {
            _themes.Remove(theme.Name);
        }
    }

    public void Remove(string themeName)
    {
        _themes.Remove(themeName);
    }

    public void Remove(Theme theme)
    {
        _themes.Remove(theme.Name);
    }

    public bool TryAdd<T>() where T : Theme, new()
    {
        return TryAdd(typeof(T));
    }

    public bool TryAdd(Type themeType)
    {
        if (!themeType.IsAssignableTo(typeof(Theme)))
        {
            throw new ArgumentException($"Theme must be of type: {typeof(Theme)}");
        }

        var theme = (Theme)Activator.CreateInstance(themeType)!;

        return TryAdd(theme);
    }

    public bool TryAdd(Theme theme)
    {
        if (_themes.ContainsKey(theme.Name))
        {
            return false;
        }

        _themes.Add(theme.Name, theme);
        return true;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _themes.Values.GetEnumerator();
    }
}