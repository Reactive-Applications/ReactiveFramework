using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReactiveFramework.Hosting.Abstraction;
using ReactiveFramework.Hosting.Abstraction.Plugins;

namespace ReactiveFramework.Hosting.InitializationServices;
internal class RuntimeServicesInitializer : IStartupService
{
    public Task OnAppInitiallization(IServiceProvider initializationServices, CancellationToken cancellationToken = default)
    {
        var runtimeServices = initializationServices.GetRequiredService<IServiceCollection>();
        runtimeServices.AddSingleton(initializationServices.GetRequiredService<IHostEnvironment>());
        runtimeServices.AddSingleton(initializationServices.GetRequiredService<IHostApplicationLifetime>());
        runtimeServices.AddSingleton(initializationServices.GetRequiredService<IHostLifetime>());
        runtimeServices.AddSingleton(initializationServices.GetRequiredService<IPluginManager>());

        return Task.CompletedTask;
    }

    public Task OnAppStart(IServiceProvider runtimeServices, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
