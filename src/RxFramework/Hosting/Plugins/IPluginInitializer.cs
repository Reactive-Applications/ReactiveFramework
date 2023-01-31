namespace RxFramework.Hosting.Plugins;

public interface IPluginInitializer
{
    Type PluginType { get; }
    void InitializePlugin(IPlugin plugin, IServiceProvider serviceProvider);
}