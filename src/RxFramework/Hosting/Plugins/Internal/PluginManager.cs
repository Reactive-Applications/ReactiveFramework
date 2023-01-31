using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace RxFramework.Hosting.Plugins.Internal;

internal sealed class PluginManager : IPluginManager
{
    private readonly IPluginCollection _plugins;
    private readonly List<PluginDescription> _loadedPlugins = new();

    private bool _disposed;

    public PluginManager(IPluginCollection plugins, IPluginInitializerCollection pluginInitializers)
    {
        _plugins = plugins;
        PluginInitializers = pluginInitializers;
    }

    public IEnumerable<PluginDescription> LoadedPlugins => _loadedPlugins;
    public IPluginInitializerCollection PluginInitializers { get; }

    private readonly Subject<PluginDescription> _pluginLoadingSubject = new();
    public IObservable<PluginDescription> WhenPluginLoading => _pluginLoadingSubject;

    private readonly Subject<PluginDescription> _pluginLoadedSubject = new();
    public IObservable<PluginDescription> WhenPluginLoaded => _pluginLoadedSubject;

    public bool AutoInitialize { get; set; } = true;

    public void InitializePlugins(IServiceProvider services)
    {
        PluginInitializers.CreateInitializers(services);
        foreach (var pluginDescription in _plugins)
        {
            var plugin = (IPlugin)services.GetUnregisteredService(pluginDescription.PluginType);

            var logger = services.GetRequiredService<ILogger<PluginManager>>();

            logger.LogDebug("Loading plugin: {PluginDescription}", pluginDescription);
            _pluginLoadingSubject.OnNext(pluginDescription);
            var initializers = PluginInitializers.GetInitializersFor(plugin);
            var trigger = _plugins.GetTriggerFor(pluginDescription.PluginType);

            trigger
                .Take(1)
                .Subscribe(_ =>
                {
                    foreach (var initializer in initializers)
                    {
                        initializer.InitializePlugin(plugin, services);
                    }
                });

            logger.LogDebug("plugin: {PluginDescription} loaded", pluginDescription);
            _loadedPlugins.Add(pluginDescription);
            _pluginLoadedSubject.OnNext(pluginDescription);
        }
        _pluginLoadingSubject.OnCompleted();
        _pluginLoadedSubject.OnCompleted();
    }

    public void RegisterServices(IServiceCollection services)
    {
        foreach (var plugin in _plugins)
        {
            plugin.PluginType.GetMethod("RegisterServices")!.Invoke(null, new[] { services });
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _pluginLoadedSubject.Dispose();
            _pluginLoadingSubject.Dispose();
        }

        _disposed = true;
    }
}