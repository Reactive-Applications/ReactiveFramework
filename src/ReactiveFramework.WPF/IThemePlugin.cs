using ReactiveFramework.WPF.Theming;
using ReactiveFramework.Hosting.Plugins.Attributes;

namespace ReactiveFramework.WPF;
public interface IThemePlugin
{
    [InvokedAtPluginInitialization]
    void RegisterThemes(IThemeCollection themes);
}
