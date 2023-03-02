using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReactiveFramework.Hosting.Abstraction.Plugins;

namespace ReactiveFramework.Hosting.Abstraction;
public interface IPluginApplication : IHost
{
    bool IsInitialized { get; }

    new IServiceProvider Services { get; }

    IPluginCollection Plugins { get; }

    ILoggingBuilder RuntimeLogging { get; }

    Task Initialize(CancellationToken cancellationToken = default);
}
