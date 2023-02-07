namespace ReactiveFramework.Hosting.Plugins;

public interface IPluginManager : IDisposable
{
    bool AutoInitialize { get; set; }

    IObservable<PluginDescription> WhenPluginLoading { get; }
    IObservable<PluginDescription> WhenPluginLoaded { get; }

    IEnumerable<PluginDescription> LoadedPlugins { get; }

    IEnumerable<PluginDescription> DiscoveredPlugins { get; }

    void RegisterPlugins(IServiceProvider registrationServices);

    void RegisterPlugin(PluginDescription plugin, IServiceProvider registrationServices);


    void InitializePlugins(IServiceProvider appServices);

    void InitializePlugin(PluginDescription plugin, IServiceProvider appServices);
}
