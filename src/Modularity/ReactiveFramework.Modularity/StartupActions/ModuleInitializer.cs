using Microsoft.Extensions.Logging;
using ReactiveFramework.Hosting.Abstraction;
using ReactiveFramework.Modularity.Abstraction;

namespace ReactiveFramework.Modularity.StartupActions;
internal class ModuleInitializer : IHostStartupAction
{
    private IModuleCatalog _modules;
    private readonly IServiceProvider _services;
    private readonly ILogger<ModuleInitializer> _logger;

    public ModuleInitializer(IModuleCatalog modules, IServiceProvider services, ILogger<ModuleInitializer> logger)
    {
        _modules = modules;
        _services = services;
        _logger = logger;
    }

    public HostStartupActionExecution ExecutionTime { get; set; } = HostStartupActionExecution.AfterHostedServicesStarted;
    public int Priority => ModuleStartupActionPriorities.ModuleInitalization;

    public async Task Execute(CancellationToken cancellation)
    {
        foreach (var module in _modules)
        {
            try
            {
                await module.StartModuleAsync(_services, cancellation)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"An error accoured while starting the {moduleName} Module", module.Name);
            }
        }
    }
}

public class ModuleStartupActionPriorities
{
    public const int ModuleInitalization = -100;
    public const int BeforeModuleInitialization = ModuleInitalization - 10; 
    public const int AfterModuleInitialization = ModuleInitalization + 10; 
}
