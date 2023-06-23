using ReactiveFramework.Hosting.Abstraction;
using ReactiveFramework.Modularity.StartupActions;
using ReactiveFramework.WPF.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace ReactiveFramework.WPF.StartupActions;
internal class WpfAppShellInitializer : IHostStartupAction
{
    private readonly IWpfThread _wpfThread;
    private readonly ISplashWindowViewModel? _splashViewModel;

    public WpfAppShellInitializer(IWpfThread wpfThread, ISplashWindowViewModel? splashViewModel)
    {
        _wpfThread = wpfThread;
        _splashViewModel = splashViewModel;
    }

    public int Priority => StartupActionPriority.ShowSplashScreen;
    public HostStartupActionExecution ExecutionTime => HostStartupActionExecution.AfterHostedServicesStarted;

    public async Task Execute(CancellationToken cancellation)
    {
        if (_splashViewModel != null)
        {
            await _wpfThread.UiDispatcher!.InvokeAsync(() =>
            {
                var splashWindow = Activator.CreateInstance(_splashViewModel.WindowType) as Window ??
                    throw new NotSupportedException("the provided splashWindowType is not valid. The splash window must be an Window and " +
                    "it must contain a parameterless constructor");

                splashWindow.DataContext = _splashViewModel;
                if (!_wpfThread.AppCreated)
                {
                    throw new UnreachableException();
                }

                _splashViewModel.SplashWindow = splashWindow;
                _wpfThread.Application.MainWindow = splashWindow;
                splashWindow.Show();
            });
            await _splashViewModel.OnAppStartAsync();
        }
    }
}

internal class WpfAppShellAfterModuleInitialization : IHostStartupAction
{
    private readonly ISplashWindowViewModel _slpashViewModel;

    public int Priority => StartupActionPriority.ShowSplashScreen + 1;
    public HostStartupActionExecution ExecutionTime => HostStartupActionExecution.AfterHostedServicesStarted;

    public WpfAppShellAfterModuleInitialization(ISplashWindowViewModel slpashViewModel)
    {
        _slpashViewModel = slpashViewModel;
    }

    public async Task Execute(CancellationToken cancellation)
    {
        if (_slpashViewModel.SplashWindow == null)
        {
            return;
        }

        await _slpashViewModel.AfterModuleInitalizationAsync().ConfigureAwait(false);
    }
}
