using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Concurrency;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace ReactiveFramework.WPF.Hosting.Internal;
internal class WPFThread : IWPFThread
{
    private readonly ManualResetEvent _lock;
    private readonly ManualResetEvent _appBuild;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHostApplicationLifetime _applicationLifetime;

    public Thread Thread { get; }
    public Application? Application { get; private set; }

    public bool AppIsRunnning { get; private set; }

    [MemberNotNullWhen(true, nameof(Application))]
    public bool AppCreated { get; private set; }
    public Dispatcher UiDispatcher { get; private set; }

    public WPFThread(IHostApplicationLifetime applicationLifetime, IServiceProvider serviceProvider)
    {
        _applicationLifetime = applicationLifetime;
        _serviceProvider = serviceProvider;
        _lock = new ManualResetEvent(false);
        _appBuild = new ManualResetEvent(false);
        Thread = new Thread(InternalThread);
        Thread.IsBackground = false;
        Thread.SetApartmentState(ApartmentState.STA);
        Thread.Start();
        applicationLifetime.ApplicationStopped.Register(() =>
        {
            if(!AppIsRunnning)
            {
                _lock.Set();
            }
            StopApplication();
        });
    }

    public void StartApplication()
    {
        _lock.Set();
    }

    public void StopApplication()
    {
        if (!AppIsRunnning || !AppCreated || UiDispatcher.HasShutdownFinished)
        {
            return;
        }
        UiDispatcher.Invoke(() => Application.Shutdown());
    }

    private void InternalThread()
    {
        UiDispatcher = Dispatcher.CurrentDispatcher;
        SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(UiDispatcher));

        Schedulers.MainScheduler = new SynchronizationContextScheduler(SynchronizationContext.Current);

        Application = _serviceProvider.GetRequiredService<Application>();
        AppCreated = true;
        _appBuild.Set();
        _lock.WaitOne();
        if (_applicationLifetime.ApplicationStopped.IsCancellationRequested)
        {
            return;
        }
        AppIsRunnning = true;
        Application.Run();
        AppIsRunnning = false;
        _applicationLifetime.StopApplication();
    }

    public Task WaitForAppBuild()
    {
        if (AppCreated)
        {
            return Task.CompletedTask;
        }

        _appBuild.WaitOne();

        return Task.CompletedTask;
    }

    public Task WaitForAppStart()
    {
        if(AppIsRunnning)
        {
            return Task.CompletedTask;
        }
        _lock.WaitOne();

        return Task.CompletedTask;
    }
}
