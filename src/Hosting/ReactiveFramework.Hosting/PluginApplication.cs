using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReactiveFramework.Hosting.Abstraction;
using ReactiveFramework.Hosting.Abstraction.Plugins;
using ReactiveFramework.Hosting.Plugins;
using System;
using System.Diagnostics.CodeAnalysis;

namespace ReactiveFramework.Hosting;
public class PluginApplication : IPluginApplication
{
    private readonly PluginHostBuilderContext _context;
    private readonly IServiceProvider _initializationServices;
    private bool _initializing;

    [MemberNotNullWhen(true, nameof(Services))]
    public bool IsInitialized { get; private set; }

    public IServiceProvider? Services { get; private set; }

    public IServiceCollection RuntimeServices { get; }

    public IPluginCollection Plugins { get; }

    internal PluginApplication(PluginHostBuilderContext context, IServiceProvider initializationServices)
    {
        _initializationServices = Services = initializationServices;
        _context = context;
        Plugins = _initializationServices.GetRequiredService<IPluginCollection>();
        RuntimeServices = _initializationServices.GetRequiredService<IServiceCollection>();
    }

    public void Dispose()
    {
    }

    // TODO: Build Runtime Services with container builder action
    // TODO: Configure Logging (copy logging options from appBuilder if option is set)
    // TODO: Register RuntimeServices from configure services actions
    public virtual async Task Initialize()
    {
        if(_initializing)
        {
            return;
        }

        _initializing = true;

        var pluginManager = _initializationServices.GetRequiredService<IPluginManager>();
        
        Plugins.MakeReadOnly();

        await pluginManager.InitializePluginsAsync(_initializationServices).ConfigureAwait(false);

        var buildServices = _initializationServices.GetRequiredService<Func<IServiceCollection, IServiceProvider>>();
        
        Services = buildServices(RuntimeServices);
        IsInitialized = true;
    }

    public virtual async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if(_context.AutoInitialize)
        {
            await Initialize().ConfigureAwait(false);
        }

        if (!IsInitialized)
        {
            throw new InvalidOperationException("call Initialize first");
        }


    }

    public virtual Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (!IsInitialized)
        {
            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }

    public static IPluginApplicationBuilder CreateBuilder(string[] args)
    {
        return new PluginApplicationBuilder(new PluginApplicationBuilderSettings()
        {
            Args = args
        });
    }
}
