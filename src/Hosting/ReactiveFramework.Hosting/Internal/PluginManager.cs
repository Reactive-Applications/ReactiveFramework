using Microsoft.Extensions.DependencyInjection;
using ReactiveFramework.Hosting.Abstraction.Plugins;
using ReactiveFramework.Hosting.Plugins;
using System.Reactive.Subjects;

namespace ReactiveFramework.Hosting.Internal;

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

    public async Task StartPluginAsync(PluginDescription plugin, IServiceProvider appServices)
    {
        _pluginLoadingSubject.OnNext(plugin);

        foreach (var startMethod in plugin.StartMethods)
        {
            var parameters = appServices.GetServices(startMethod.GetParameters()
               .Select(p => p.ParameterType).Where(t => !t.IsAssignableTo(typeof(CancellationToken))))
               .ToArray();

            if (startMethod.ReturnType.IsAssignableTo(typeof(Task)))
            {
                await ((Task)startMethod.Invoke(plugin.Instance, parameters)!).ConfigureAwait(false);
            }
            else
            {
                startMethod.Invoke(plugin.Instance, parameters);
            }
        }

        _pluginLoadedSubject.OnNext(plugin);        
    }

    public async Task StartPluginsAsync(IServiceProvider appServices)
    {
        foreach (var plugin in _loadedPlugins)
        {
            await StartPluginAsync(plugin, appServices).ConfigureAwait(false);
        }
    }
    public async Task InitializePluginAsync(PluginDescription pluginDescription, IServiceProvider initializationServices, CancellationToken cancellationToken = default)
    {
        pluginDescription.InitializeDescriptor();

        foreach (var startMethod in pluginDescription.InitializeMethods)
        {
            var parameters = initializationServices.GetServices(startMethod.GetParameters()
                .Select(p => p.ParameterType).Where(t => !t.IsAssignableTo(typeof(CancellationToken))))
                .ToArray();

            if (startMethod.ReturnType.IsAssignableTo(typeof(Task)))
            {
                await ((Task)startMethod.Invoke(pluginDescription.Instance, parameters)!).ConfigureAwait(false);
            }
            else
            {
                startMethod.Invoke(pluginDescription.Instance, parameters);
            }
        }

        _loadedPlugins.Add(pluginDescription);
    }

    public async Task InitializePluginsAsync(IServiceProvider initializationServices, CancellationToken cancellationToken = default)
    {
        foreach (var pluginDescription in _plugins)
        {
            await InitializePluginAsync(pluginDescription, initializationServices, cancellationToken).ConfigureAwait(false);
        }
    }
}