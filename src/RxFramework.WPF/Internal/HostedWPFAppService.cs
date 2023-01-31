using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RxFramework.Hosting.Plugins;
using RxFramework.WPF.Hosting;
using RxFramework.WPF.Theming;
using RxFramework.WPF.ViewComposition;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace RxFramework.WPF.Internal;
internal class HostedWPFAppService : IHostedService
{

    private readonly IServiceProvider _services;
    private readonly WPFAppContext _appContext;
    private readonly ILogger _logger;
    private Application? _app;

    public HostedWPFAppService(IServiceProvider services, WPFAppContext context, ILogger<HostedWPFAppService> logger)
    {
        _services = services;
        _appContext = context;
        _logger = logger;

        var initializers = _services.GetService<IPluginInitializerCollection>();

        initializers?.Add<UiPluginInitializer>();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.CompletedTask;
        }

        var thread = new Thread(WpfThread);
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_appContext.IsRunning)
        {
            _appContext.IsRunning = false;
            _logger.LogInformation("Shutdown app because the host was stoped");
            await RxApp.Dispatcher.InvokeAsync(() => _app?.Shutdown());
        }
    }

    private void WpfThread()
    {
        SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));

        var field = typeof(Schedulers).GetField("_uiScheduler", BindingFlags.NonPublic | BindingFlags.Static);

        var scheduler = new SynchronizationContextScheduler(SynchronizationContext.Current);

        field!.SetValue(null, scheduler);

        var app = _services.GetRequiredService<Application>();
        _app = app;
        app.Exit += (s, e) => Shutdown();
        RxApp.Dispatcher = Dispatcher.CurrentDispatcher;

        app.Resources = new ResourceDictionary();

        _logger.LogInformation("Starting application");

        var resActions = _services.GetService<IEnumerable<Action<ResourceDictionary>>>();
        resActions ??= Enumerable.Empty<Action<ResourceDictionary>>();
        foreach (var action in resActions)
        {
            action(app.Resources);
        }

        _services.GetRequiredService<IThemeManager>().Initialize();
        _appContext.IsRunning = true;

        app.Run();
    }

    private void Shutdown()
    {
        if (!_appContext.IsRunning)
        {
            return;
        }

        _appContext.IsRunning = false;

        if (!_appContext.IsLifetimeLinked)
        {
            return;
        }
        _logger.LogInformation("Shutting down host because the app was closed");
        _services.GetService<IHostApplicationLifetime>()?.StopApplication();
    }
}
