using RxFramework.Hosting.Plugins.Attributes;
using RxFramework.WPF.Theming;

namespace RxFramework.WPF;
public interface IThemePlugin
{
    [InvokedAtPluginInitialization]
    void RegisterThemes(IThemeCollection themes);
}
