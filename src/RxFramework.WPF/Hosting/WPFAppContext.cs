using System.Windows;

namespace RxFramework.WPF.Hosting;
public class WPFAppContext
{
    public ShutdownMode ShutdownMode { get; set; } = ShutdownMode.OnMainWindowClose;

    public bool IsLifetimeLinked { get; set; } = true;
    public bool IsRunning { get; internal set; }

    internal WPFAppContext()
    {
    }
}
