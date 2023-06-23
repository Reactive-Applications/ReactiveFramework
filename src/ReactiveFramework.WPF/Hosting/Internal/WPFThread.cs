using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReactiveFramework.WPF.ViewAdapters;
using ReactiveFramework.WPF.ViewComposition;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Concurrency;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace ReactiveFramework.WPF.Hosting.Internal;
internal class WpfThread : IWpfThread
{
    private readonly ManualResetEvent _lock;
    private readonly ManualResetEvent _appStartedEventlock;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHostApplicationLifetime _applicationLifetime;

    private bool _externalStop;

    public Thread Thread { get; }
    public Application? Application { get; private set; }

    public bool AppIsRunnning { get; private set; }

    [MemberNotNullWhen(true, nameof(Application))]
    [MemberNotNullWhen(true, nameof(UiDispatcher))]
    public bool AppCreated { get; private set; }
    public Dispatcher? UiDispatcher { get; private set; }

    public WpfThread(IHostApplicationLifetime applicationLifetime, IServiceProvider serviceProvider)
    {
        _applicationLifetime = applicationLifetime;
        _serviceProvider = serviceProvider;
        _lock = new ManualResetEvent(false);
        _appStartedEventlock = new(false);
        Thread = new Thread(InternalThread)
        {
            IsBackground = false
        };
        Thread.SetApartmentState(ApartmentState.STA);
        Thread.Start();
    }

    private void InternalThread()
    {
        UiDispatcher = Dispatcher.CurrentDispatcher;
        SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(UiDispatcher));

        Schedulers.MainScheduler = new SynchronizationContextScheduler(SynchronizationContext.Current);

        Application = _serviceProvider.GetRequiredService<Application>();
        AppCreated = true;
        _lock.WaitOne();
        if (_applicationLifetime.ApplicationStopped.IsCancellationRequested)
        {
            return;
        }

        var viewCollection = _serviceProvider.GetRequiredService<IViewCollection>();
        var viewAdapters = _serviceProvider.GetRequiredService<IViewAdapterCollection>();
        var executingAssembly = Assembly.GetEntryAssembly()!;

        viewCollection.AddViewsFromAssembly(executingAssembly);
        viewAdapters.AddAdaptersFromAssembly(executingAssembly);
        viewAdapters.AddAdaptersFromAssembly(Assembly.GetAssembly(typeof(ContentControlAdapter))!);

        _appStartedEventlock.Set();
        AppIsRunnning = true;
        Application.Run();
        AppIsRunnning = false;

        if (!_externalStop)
        {
            _applicationLifetime.StopApplication();
        }
    }

    public Task WaitForAppStart()
    {
        _appStartedEventlock.WaitOne();
        return Task.CompletedTask;
    }

    public Task StartAsync(CancellationToken cancellation)
    {
        _externalStop = false;
        _lock.Set();
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellation)
    {
        if (!AppIsRunnning)
        {
            _lock.Set();
        }

        if (!AppIsRunnning || !AppCreated || UiDispatcher.HasShutdownFinished)
        {
            return;
        }
        _externalStop = true;
        await UiDispatcher.InvokeAsync(() => Application.Shutdown());
    }
}
