using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Threading;

namespace ReactiveFramework.WPF.Hosting;
public interface IWPFThread
{
    bool AppIsRunnning { get; }

    [MemberNotNullWhen(true, nameof(Application))]
    bool AppCreated { get; }

    Thread Thread { get; }
    Application? Application { get; }
    Dispatcher UiDispatcher { get; }


    void StartApplication();
    void StopApplication();
    Task WaitForAppBuild();
    Task WaitForAppStart();
}
