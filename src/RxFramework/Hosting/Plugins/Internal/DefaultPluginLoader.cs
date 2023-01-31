namespace RxFramework.Hosting.Plugins.Internal;

internal class DefaultPluginInitializer : IPluginInitializer
{
    public Type PluginType => typeof(IPlugin);

    public void InitializePlugin(IPlugin plugin, IServiceProvider serviceProvider)
    {
        plugin.Initialize();
    }
}
