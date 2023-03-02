namespace ReactiveFramework.Hosting.Abstraction.Plugins;

public interface IPluginManager : IDisposable
{
    IObservable<PluginDescription> WhenPluginLoading { get; }
    IObservable<PluginDescription> WhenPluginLoaded { get; }

    IEnumerable<PluginDescription> LoadedPlugins { get; }

    IEnumerable<PluginDescription> DiscoveredPlugins { get; }

    Task InitializePluginAsync(PluginDescription plugin, IServiceProvider initializationServices, CancellationToken cancellationToken);
    
    Task InitializePluginsAsync(IServiceProvider initializationServices, CancellationToken cancellationToken);

    Task StartPluginAsync(PluginDescription plugin, IServiceProvider appServices);

    Task StartPluginsAsync(IServiceProvider appServices);
}
