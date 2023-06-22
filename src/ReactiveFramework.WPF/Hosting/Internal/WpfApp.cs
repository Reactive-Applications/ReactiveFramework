using Microsoft.Extensions.Hosting;
using System.Windows;

namespace ReactiveFramework.WPF.Hosting.Internal;
public class WpfApp : IHostedService
{
    private Window? _splashWindow;
    private ISplashWindowViewModel? _splashViewModel;
    private readonly IWpfThread _wpfThread;

    public WpfApp(IWpfThread wpfThread)
    {
        _wpfThread = wpfThread;
    }

    public async Task StartAsync(CancellationToken cancellation)
    {
        await _wpfThread.StartAsync(cancellation).ConfigureAwait(false);
        await _wpfThread.WaitForAppStart().ConfigureAwait(false);
    }

    public Task StopAsync(CancellationToken cancellation)
    {
        return _wpfThread.StopAsync(cancellation);
    }
}
