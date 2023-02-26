using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReactiveFramework.Hosting.Abstraction.Plugins;

namespace ReactiveFramework.Hosting.Abstraction;
public interface IPluginApplication : IHost
{
    bool IsInitialized { get; }

    IServiceCollection RuntimeServices { get; }

    IPluginCollection Plugins { get; }

    Task Initialize();
}
