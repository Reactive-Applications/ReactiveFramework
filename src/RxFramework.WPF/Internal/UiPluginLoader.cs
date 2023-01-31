using Microsoft.Extensions.DependencyInjection;
using RxFramework.Hosting.Plugins;
using RxFramework.WPF.ViewComposition;

namespace RxFramework.WPF.Internal;
internal class UiPluginInitializer : IPluginInitializer
{
    public Type PluginType => typeof(IUiPlugin);

    public void InitializePlugin(IPlugin plugin, IServiceProvider serviceProvider)
    {
        InitializeUIPlugin((IUiPlugin)plugin, serviceProvider);
    }

    private void InitializeUIPlugin(IUiPlugin plugin, IServiceProvider serviceProvider)
    {
        var viewCollection = serviceProvider.GetRequiredService<IViewCollection>();
        viewCollection.AddViewsFromAssembly(plugin.GetType().Assembly);
    }
}
