using Microsoft.Extensions.Hosting;

namespace ReactiveFramework.WPF.Hosting;
public class WPFLifetime : IHostLifetime
{
    private readonly IWPFThread _thread;

    public WPFLifetime(IWPFThread wpfThread)
    {
        _thread = wpfThread;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public async Task WaitForStartAsync(CancellationToken cancellationToken)
    {
        await _thread.WaitForAppStart();
    }
}
