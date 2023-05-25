using Microsoft.Extensions.DependencyInjection;
using ReactiveFramework.Modularity.Abstraction;

namespace ReactiveFramework.Modularity.InitializationServices;
internal class ModuleInitializer : IStartupService
{
    private IModuleCatalog? _pluginManager;
    public Task OnAppInitialization(IServiceProvider initializationServices, CancellationToken cancellationToken = default)
    {
        _pluginManager = initializationServices.GetRequiredService<IModuleManager>();
        var pluginCollecttion = initializationServices.GetRequiredService<IModuleCatalog>();
        pluginCollecttion.MakeReadOnly();
        return _pluginManager.InitializeModulesAsync(initializationServices, cancellationToken);
    }

    public Task OnAppStart(IServiceProvider runtimeServices, CancellationToken cancellationToken = default)
    {
        return _pluginManager == null
            ? Task.FromException(new InvalidOperationException("Initializer not called on app initalization"))
            : _pluginManager.StartModulesAsync(runtimeServices);
    }
}
