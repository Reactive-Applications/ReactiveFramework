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
    private ILogger<PluginApplication> _logger;

    private IEnumerable<IHostedService> _hostedServices = Enumerable.Empty<IHostedService>();
    private IHostLifetime? _lifetime;
    private ApplicationLifetime? _applicationLifetime;

    private bool _stopCalled;

    private IServiceProvider? _runTimeServices;
    private List<IStartupService> _startupServices = new();

    private bool _initializing;

    [MemberNotNullWhen(true, nameof(Services))]
    public bool IsInitialized { get; private set; }

    public IServiceProvider Services => _runTimeServices ?? throw new InvalidOperationException("call Initialize first");

    public IPluginCollection Plugins { get; }

    public ILoggingBuilder RuntimeLogging { get; }

    internal PluginApplication(PluginHostBuilderContext context, IServiceProvider initializationServices)
    {
        _initializationServices = initializationServices;
        _context = context;
        Plugins = _initializationServices.GetRequiredService<IPluginCollection>();
        RuntimeLogging = _initializationServices.GetRequiredService<ILoggingBuilder>();
        _logger = _initializationServices.GetRequiredService<ILogger<PluginApplication>>();
    }

    public void Dispose()
    {
    }

    public virtual async Task Initialize(CancellationToken cancellationToken = default)
    {
        if (_initializing)
        {
            return;
        }

        _initializing = true;

        _logger.LogDebug("Initializing app");

        var startupServices = _initializationServices.GetServices<IStartupService>();

        foreach (var startupService in startupServices)
        {
            try
            {
                await startupService.OnAppInitiallization(_initializationServices, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Startupservice '{serviceName}' faild on app initialization", startupService.GetType().FullName);
                continue;
            }
            _startupServices.Add(startupService);
        }

        var buildServices = _initializationServices.GetRequiredService<Func<IServiceCollection, IServiceProvider>>();
        var runtimeServiceCollection = _initializationServices.GetRequiredService<IServiceCollection>();
        _runTimeServices = buildServices(runtimeServiceCollection);

        IsInitialized = true;
        _logger.LogDebug("App initialized");
    }

    public virtual async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (_context.AutoInitialize && !IsInitialized)
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

        _logger.LogDebug("Starting App");

        using var combinedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _applicationLifetime.ApplicationStopping);
        var token = combinedCancellationTokenSource.Token;

        await _lifetime.WaitForStartAsync(cancellationToken).ConfigureAwait(false);

        token.ThrowIfCancellationRequested();

        _hostedServices = Services.GetServices<IHostedService>();

        foreach (var startupService in _startupServices)
        {
            try
            {
                await startupService.OnAppStart(Services, cancellationToken).ConfigureAwait(false); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Startupservice '{serviceName}' faild on app start", startupService.GetType().FullName);
            }
        }

        foreach (var hostedService in _hostedServices)
        {
            await hostedService.StartAsync(token).ConfigureAwait(false);

            if (hostedService is BackgroundService backgroundService)
            {
                _ = TryExecuteBackgroundServiceAsync(backgroundService);
            }
        }

        _applicationLifetime.NotifyStarted();
        _logger.LogDebug("App started");
    }

    public async virtual Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (!IsInitialized)
        {
            return;
        }

        _logger.LogDebug("Application stopping");
        _applicationLifetime!.StopApplication();

        var exceptions = new List<Exception>();

        foreach (var hostedService in _hostedServices)
        {
            try
            {
                await hostedService.StopAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }
        _applicationLifetime.NotifyStopped();

        try
        {
            await _lifetime!.StopAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            exceptions.Add(ex);
        }

        if (exceptions.Count > 0)
        {
            var ex = new AggregateException("One or more hosted services failed to stop.", exceptions);
            _logger.LogError(ex, "Application stopped with exceptions");
            throw ex;
        }

        _logger.LogDebug("Application stopped");
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
        var backgroundTask = backgroundService.ExecuteTask;
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
