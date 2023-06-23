using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Threading;

namespace ReactiveFramework.WPF.Hosting;
public interface IWpfThread
{
    bool AppIsRunnning { get; }

    [MemberNotNullWhen(true, nameof(Application))]
    [MemberNotNullWhen(true, nameof(UiDispatcher))]
    bool AppCreated { get; }

    Thread Thread { get; }
    Application? Application { get; }
    Dispatcher? UiDispatcher { get; }


    Task StartAsync(CancellationToken cancellation);
    Task StopAsync(CancellationToken cancellation);

    Task WaitForAppStart();
}
