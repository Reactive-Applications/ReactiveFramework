using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReactiveFramework.Hosting.Abstraction;
using ReactiveFramework.Hosting.Abstraction.Plugins;
using System.Diagnostics.CodeAnalysis;

namespace ReactiveFramework.Hosting;
public class PluginApplication : IPluginApplication
{
    private readonly PluginHostBuilderContext _context;
    private readonly IServiceProvider _initializationServices;
    
    private IEnumerable<IHostedService> _hostedServices = Enumerable.Empty<IHostedService>();
    private IHostLifetime _lifetime;
    private ApplicationLifetime _applicationLifetime;
    private IPluginManager _pluginManager;
    
    private bool _stopCalled;
    private ILogger<PluginApplication> _logger;
    private IOptions<HostOptions> _options;

    private IServiceProvider? _runTimeServices;

    private bool _initializing;

    [MemberNotNullWhen(true, nameof(Services))]
    public bool IsInitialized { get; private set; }

    public IServiceProvider Services => _runTimeServices ?? throw new InvalidOperationException("call Initialize first");

    public IPluginCollection Plugins { get; }

    internal PluginApplication(PluginHostBuilderContext context, IServiceProvider initializationServices)
    {
        _initializationServices = initializationServices;
        _context = context;
        Plugins = _initializationServices.GetRequiredService<IPluginCollection>();
    }

    public void Dispose()
    {
    }

    public virtual async Task Initialize(CancellationToken cancellationToken = default)
    {
        if(_initializing)
        {
            return;
        }

        _initializing = true;

        _pluginManager = _initializationServices.GetRequiredService<IPluginManager>();
        _initializationServices.GetRequiredService<ILoggingBuilder>();
        
        Plugins.MakeReadOnly();

        await _pluginManager.InitializePluginsAsync(_initializationServices, cancellationToken).ConfigureAwait(false);

        var buildServices = _initializationServices.GetRequiredService<Func<IServiceCollection, IServiceProvider>>();

        var runtimeServiceCollection = _initializationServices.GetRequiredService<IServiceCollection>();

        runtimeServiceCollection.AddSingleton(_initializationServices.GetRequiredService<IHostEnvironment>());
        runtimeServiceCollection.AddSingleton(_initializationServices.GetRequiredService<IHostApplicationLifetime>());
        runtimeServiceCollection.AddSingleton(_initializationServices.GetRequiredService<IHostLifetime>());

        _runTimeServices = buildServices(runtimeServiceCollection);

        IsInitialized = true;
    }

    public virtual async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if(_context.AutoInitialize && !IsInitialized)
        {
            await Initialize(cancellationToken).ConfigureAwait(false);
        }

        if (!IsInitialized)
        {
            throw new InvalidOperationException("call Initialize first");
        }
        _lifetime = Services.GetRequiredService<IHostLifetime>();
        _applicationLifetime = (ApplicationLifetime)Services.GetRequiredService<IHostApplicationLifetime>();
        _logger = Services.GetRequiredService<ILogger<PluginApplication>>();

        _logger.LogDebug("Hosting staring");

        using var combinedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _applicationLifetime.ApplicationStopping);
        var token = combinedCancellationTokenSource.Token;

        await _lifetime.WaitForStartAsync(cancellationToken).ConfigureAwait(false);

        await _pluginManager.StartPluginsAsync(Services);
        token.ThrowIfCancellationRequested();

        _hostedServices = Services.GetServices<IHostedService>();

        foreach (var hostedService in _hostedServices)
        {
            await hostedService.StartAsync(token).ConfigureAwait(false);

            if(hostedService is BackgroundService backgroundService) 
            {
                _ = TryExecuteBackgroundServiceAsync(backgroundService);
            }
        }

        _applicationLifetime.NotifyStarted();
        _logger.LogDebug("Hosting started");
    }

    public virtual Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (!IsInitialized)
        {
            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }

    public static IPluginApplicationBuilder CreateDefaultBuilder(string[]? args = null)
    {
        return new PluginApplicationBuilder(new PluginApplicationBuilderSettings()
        {
            Args = args
        });
    }

    public static IPluginApplicationBuilder CreateBuilder(PluginApplicationBuilderSettings settings)
        => new PluginApplicationBuilder(settings);

    private async Task TryExecuteBackgroundServiceAsync(BackgroundService backgroundService)
    {
        // backgroundService.ExecuteTask may not be set (e.g. if the derived class doesn't call base.StartAsync)
        Task? backgroundTask = backgroundService.ExecuteTask;
        if (backgroundTask == null)
        {
            return;
        }

        try
        {
            await backgroundTask.ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // When the host is being stopped, it cancels the background services.
            // This isn't an error condition, so don't log it as an error.
            if (_stopCalled && backgroundTask.IsCanceled && ex is OperationCanceledException)
            {
                return;
            }

            _logger.LogError(ex, "Background service: {backgroundService} throwed an exception", backgroundService);
        }
    }
}
