using ReactiveFramework.Hosting;
using System.Windows;
using System.Windows.Diagnostics;

namespace ReactiveFramework.WPF.Hosting;

public interface IHostedWPFAppBuilder : IHostedPluginAppBuilder
{
    IHostedWPFAppBuilder AddResources(string path);

    IHostedWPFAppBuilder AddResources(ResourceDictionary resources);

    IHostedWPFAppBuilder UseApp<TApp>()
        where TApp : Application;

    IHostedWPFAppBuilder UseApp<TApp>(Func<IServiceProvider, TApp> appFactory)
        where TApp : Application;

    IHostedWPFAppBuilder UseShell<TShell>()
        where TShell : Window;

    IHostedWPFAppBuilder UseSplashScreen<TSplashScreen>()
        where TSplashScreen : Window;
}