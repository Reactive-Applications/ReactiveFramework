using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReactiveFramework.Hosting.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveFramework.Hosting.Internal;
internal class RxHost : IHost
{
    private readonly ILogger<RxHost> _logger;
    private readonly IHostLifetime _hostLifetime;
    private readonly ApplicationLifetime _applicationLifetime;
    private readonly IHostEnvironment _hostEnvironment;
    private IEnumerable<IHostedService>? _hostedServices;

    private volatile bool _stopCalled;

    public IServiceProvider Services { get; }

    public RxHost(IServiceProvider services,
                  IHostEnvironment hostEnvironment,
                  IHostApplicationLifetime applicationLifetime,
                  ILogger<RxHost> logger,
                  IHostLifetime hostLifetime)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(applicationLifetime);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(hostLifetime);

        Services = services;

        _applicationLifetime = (applicationLifetime as ApplicationLifetime)!;
        _hostEnvironment = hostEnvironment;

        if (_applicationLifetime is null)
        {
            throw new ArgumentException("HostApplicationLifetime type not supported");
        }
        _logger = logger;
        _hostLifetime = hostLifetime;
    }

    public void Dispose()
    {
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Starting App");
        using var combinedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _applicationLifetime.ApplicationStopping);
        var token = combinedCancellationTokenSource.Token;

        await _hostLifetime.WaitForStartAsync(cancellationToken).ConfigureAwait(false);
        token.ThrowIfCancellationRequested();


        var startupActions = Services.GetHostStartupActions()
            .OrderBy(a => a.Priority);
        _hostedServices = Services.GetServices<IHostedService>();

        foreach (var action in startupActions
            .Where(a => a.ExecutionTime == HostStartupActionExecution.BeforeHostedServicesStarted)) 
        {
            try
            {
                await action.Execute(token).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "StartupAction '{actionName}' faild on app start", action.GetType().FullName);
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

        foreach (var action in startupActions
            .Where(a => a.ExecutionTime == HostStartupActionExecution.AfterHostedServicesStarted))
        {
            try
            {
                await action.Execute(token).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "startup action '{actionName}' faild on app start", action.GetType().FullName);
            }
        }

    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
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
            await _hostLifetime.StopAsync(cancellationToken).ConfigureAwait(false);
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
