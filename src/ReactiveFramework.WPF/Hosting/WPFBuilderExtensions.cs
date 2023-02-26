using System.Windows;

namespace ReactiveFramework.WPF.Hosting;
public static class WPFBuilderExtensions
{
    public static IWPFAppBuilder UseApp<TApp>(this IWPFAppBuilder builder)
        where TApp : Application, new()
    {
        return builder;
    }

    public static IWPFAppBuilder UseApp<TApp>(this IWPFAppBuilder builder,Func<IServiceProvider, TApp> appFactory)
        where TApp : Application, new()
    {
        return builder;
    }

    public static IWPFAppBuilder UseSplashWindow<TSplashScreen>(this IWPFAppBuilder builder)
        where TSplashScreen : Window
    {
        return builder;
    }

    public static IWPFAppBuilder UseShellWindow<TShellWindow>(this IWPFAppBuilder builder)
        where TShellWindow : Window
    {
        return builder;
    }

}
