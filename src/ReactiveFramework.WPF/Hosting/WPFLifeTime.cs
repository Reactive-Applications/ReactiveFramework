using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;

namespace ReactiveFramework.WPF.Hosting;
public class WpfLifetime : IHostLifetime
{
    private readonly IWpfThread _thread;

    public WpfLifetime(IWpfThread wpfThread)
    {
        _thread = wpfThread;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task WaitForStartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
