using System.Windows;

namespace RxFramework.WPF.Theming;
public interface IThemeManager
{
    Theme ActiveTheme { get; }
    IThemeCollection Themes { get; }
    bool UseSystemDefault { get; set; }

    void ChangeTheme(Theme theme);
    void ChangeTheme(string name);

    void SetDefaultTheme<T>() where T : Theme, new();

    void Initialize();
}
