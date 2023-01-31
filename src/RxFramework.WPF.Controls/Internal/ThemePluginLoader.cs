using Microsoft.Extensions.DependencyInjection;
using RxFramework.Hosting.Plugins;
using RxFramework.WPF.Theming;
using System;

namespace RxFramework.WPF.FluentControls.Internal;
internal class ThemePluginLoader : IPluginInitializer
{
    public Type PluginType => typeof(IThemePlugin);

    public void InitializePlugin(IPlugin plugin, IServiceProvider serviceProvider)
    {
        InitializeThemePlugin((IThemePlugin)plugin, serviceProvider);
    }

    private void InitializeThemePlugin(IThemePlugin themePlugin, IServiceProvider serviceProvider)
    {
        themePlugin.RegisterThemes(serviceProvider.GetRequiredService<IThemeCollection>());
    }
}
