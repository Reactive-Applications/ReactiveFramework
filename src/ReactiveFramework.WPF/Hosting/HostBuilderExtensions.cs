using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using ReactiveFramework.Hosting.Abstraction;
using ReactiveFramework.Modularity.Extensions;
using ReactiveFramework.WPF.Hosting.Internal;
using ReactiveFramework.WPF.Internal;
using ReactiveFramework.WPF.StartupActions;
using ReactiveFramework.WPF.ViewComposition;
using System.Windows;

namespace ReactiveFramework.WPF.Hosting;
public static class HostBuilderExtensions
{
    public static IRxHostBuilder UseWPF(this IRxHostBuilder builder)
    {
        SetupBaseApp(builder);
        builder.Services.TryAddSingleton<Application, Application>();
        return builder;
    }

    public static IRxHostBuilder ConfigureWpfApp<TApplication>(this IRxHostBuilder builder)
        where TApplication : Application
    {

        SetupBaseApp(builder);
        builder.Services.TryAddSingleton<Application, TApplication>();
        return builder;
    }

    public static IRxHostBuilder UseSplashWindow<TViewModel>(this IRxHostBuilder builder)
        where TViewModel : class, ISplashWindowViewModel
    {
        builder.Services.AddSingleton<ISplashWindowViewModel, TViewModel>();
        return builder;
    }

    private static void SetupBaseApp(IRxHostBuilder builder)
    {
        var adapterCollection = new ViewAdapterCollection();
        var viewCollection = new ViewCollection();

        builder.Services.AddHostedService<WpfApp>();

        builder.Services.AddStartupActions<WpfAppShellInitializer>();
        builder.Services.AddStartupActions<WpfAppShellAfterModuleInitialization>();
        builder.Services.AddStartupActions<WpfAppInitializer>();
        
        builder.Services.AddSingleton<IHostLifetime, WpfLifetime>();
        builder.Services.AddSingleton<IViewAdapterCollection>(adapterCollection);
        builder.Services.AddSingleton<IViewCollection>(viewCollection);

        builder.Services.TryAddSingleton<IWpfThread, WpfThread>();
        builder.Services.TryAddSingleton<IViewCompositionService, ViewCompositionService>();
        builder.Services.TryAddSingleton<IViewProvider, ViewProvider>();
        builder.Services.TryAddSingleton<INavigationService, NavigationService>();
        builder.Services.TryAddSingleton<IViewCompositionService, ViewCompositionService>();

        builder.UseModularity();
        builder.UseModuleRegistrationContextBuilder(new WpfModuleContextBuilder(viewCollection, adapterCollection));
    }
}
