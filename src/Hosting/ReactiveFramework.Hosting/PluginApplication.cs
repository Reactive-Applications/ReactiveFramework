using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReactiveFramework.Hosting.Abstraction;
using ReactiveFramework.Hosting.Abstraction.Plugins;
using ReactiveFramework.Hosting.Plugins;
using System.Diagnostics.CodeAnalysis;

namespace ReactiveFramework.Hosting;
public class PluginApplication : IPluginApplication
{
    private readonly IHostBuilder _hostBuilder;
    private readonly IServiceProvider _initializationServices;

    private IHost? _host;

    [MemberNotNullWhen(true, nameof(_host))]
    public bool IsInitialized { get; private set; }

    public IServiceProvider Services { get; private set; }

    public IServiceCollection RuntimeServices { get; }

    public IPluginCollection Plugins { get; }

    internal PluginApplication(IHostBuilder hostBuilder, IServiceProvider initializationServices)
    {
        RuntimeServices = new ServiceCollection();
        _initializationServices = Services = initializationServices;
        _hostBuilder = hostBuilder;
        Plugins = _initializationServices.GetRequiredService<IPluginCollection>();
    }

    public void Dispose()
    {
    }

    public virtual async Task Initialize()
    {
        Plugins.MakeReadOnly();

        var pluginManager = _initializationServices.GetRequiredService<IPluginManager>();

        await pluginManager.RegisterPluginsAsync(_initializationServices).ConfigureAwait(false);

        _hostBuilder.ConfigureServices(services =>
        {
            foreach (var service in RuntimeServices)
            {
                services.Add(service);
            }
        });

        _host = _hostBuilder.Build();
        Services = _host.Services;

        IsInitialized = true;
    }

    public virtual Task StartAsync(CancellationToken cancellationToken = default)
    {
        return !IsInitialized ? throw new InvalidOperationException("Application not Initialized") : _host.StartAsync(cancellationToken);
    }

    public virtual Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (!IsInitialized)
        {
            return Task.CompletedTask;
        }

        return _host.StopAsync(cancellationToken);
    }
}
