using Microsoft.Extensions.DependencyInjection;
using System.Reactive.Subjects;

namespace RxFramework.Hosting.Plugins.Internal;

internal sealed class PluginManager : IPluginManager
{
    private readonly IPluginCollection _plugins;
    private readonly List<PluginDescription> _loadedPlugins = new();

    private bool _disposed;

    public PluginManager(IPluginCollection plugins)
    {
        _plugins = plugins;
    }

    public IEnumerable<PluginDescription> LoadedPlugins => _loadedPlugins;
    public IEnumerable<PluginDescription> DiscoveredPlugins => _plugins;

    private readonly Subject<PluginDescription> _pluginLoadingSubject = new();
    public IObservable<PluginDescription> WhenPluginLoading => _pluginLoadingSubject;

    private readonly Subject<PluginDescription> _pluginLoadedSubject = new();
    public IObservable<PluginDescription> WhenPluginLoaded => _pluginLoadedSubject;
    public bool AutoInitialize { get; set; } = true;

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

    public void RegisterPlugins(IServiceProvider registrationServices)
    {
        foreach (var plugin in DiscoveredPlugins)
        {
            plugin.RegisterPlugin(registrationServices);
        }
    }

    public void RegisterPlugin(PluginDescription plugin, IServiceProvider registrationServices)
    {
        plugin.RegisterPlugin(registrationServices);
    }

    public void InitializePlugins(IServiceProvider appServices)
    {
        foreach (var plugin in DiscoveredPlugins)
        {
            InitializePlugin(plugin, appServices);
        }
    }

    public void InitializePlugin(PluginDescription plugin, IServiceProvider appServices)
    {
        _pluginLoadingSubject.OnNext(plugin);
        plugin.InitializePlugin(appServices);
        _pluginLoadedSubject.OnNext(plugin);
    }
}