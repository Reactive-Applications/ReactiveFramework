namespace ReactiveFramework.WPF.Theming;
public interface IThemeCollection : IEnumerable<Theme>
{
    void Add<T>() where T : Theme, new();
    void Add(Type themeType);
    void Add(Theme theme);

    bool TryAdd<T>() where T : Theme, new();
    bool TryAdd(Type themeType);
    bool TryAdd(Theme theme);

    void Remove<T>() where T : Theme, new();
    void Remove(Type themeType);
    void Remove(string themeName);
    void Remove(Theme theme);

    Theme GetTheme<T>();
    Theme GetTheme(Type themeType);
    Theme GetTheme(string themeName);
    Theme GetTheme(ThemeType themeType);
}
