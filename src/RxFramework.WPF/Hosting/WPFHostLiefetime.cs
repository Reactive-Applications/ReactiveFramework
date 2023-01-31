using Microsoft.Extensions.Hosting;

namespace RxFramework.WPF.Hosting;

public abstract class WPFHostLiefetime : IHostLifetime
{
    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task WaitForStartAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}