using Microsoft.Extensions.DependencyInjection;
using ReactiveFramework.Hosting.Abstraction;
using ReactiveFramework.Hosting.Abstraction.Plugins;

namespace ReactiveFramework.Hosting.InitializationServices;
internal class PluginInitializer : IStartupService
{
    private IPluginManager? _pluginManager;
    public Task OnAppInitiallization(IServiceProvider initializationServices, CancellationToken cancellationToken = default)
    {
        _pluginManager = initializationServices.GetRequiredService<IPluginManager>();
        var pluginCollecttion = initializationServices.GetRequiredService<IPluginCollection>();
        pluginCollecttion.MakeReadOnly();
        return _pluginManager.InitializePluginsAsync(initializationServices, cancellationToken);
    }

    public Task OnAppStart(IServiceProvider runtimeServices, CancellationToken cancellationToken = default)
    {
        return _pluginManager == null
            ? Task.FromException(new InvalidOperationException("Initializer not called on app initalization"))
            : _pluginManager.StartPluginsAsync(runtimeServices);
    }
}
