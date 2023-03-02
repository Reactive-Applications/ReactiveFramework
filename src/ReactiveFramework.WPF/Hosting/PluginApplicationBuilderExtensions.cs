using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Windows.Themes;
using ReactiveFramework.Hosting;
using ReactiveFramework.Hosting.Abstraction;
using ReactiveFramework.WPF.Hosting.Internal;
using ReactiveFramework.WPF.Internal;
using ReactiveFramework.WPF.ViewComposition;
using System.Windows;

namespace ReactiveFramework.WPF.Hosting;
public static class PluginApplicationBuilderExtensions
{
    public static IPluginApplicationBuilder UseWPF(this IPluginApplicationBuilder builder)
    {
        builder.InitializationServices.TryAddSingleton<IViewCollection, ViewCollection>();
        builder.InitializationServices.TryAddSingleton<Application, Application>();
        builder.InitializationServices.TryAddSingleton<IViewAdapterCollection, ViewAdapterCollection>();
        builder.InitializationServices.TryAddSingleton<IWPFThread, WPFThread>();
        builder.InitializationServices.AddSingleton<IHostLifetime, WPFLifetime>();
        builder.InitializationServices.AddStartupService<WPFAppInitializer>();

        builder.RuntimeServices.TryAddSingleton<IViewCompositionService, ViewCompositionService>();
        builder.RuntimeServices.TryAddSingleton<IViewAdapterCollection, ViewAdapterCollection>();
        builder.RuntimeServices.TryAddSingleton<IViewProvider, ViewProvider>();
        builder.RuntimeServices.TryAddSingleton<INavigationService, NavigationService>();
        builder.RuntimeServices.TryAddSingleton<IViewCompositionService, ViewCompositionService>();

        return builder;
    }

    public static IPluginApplicationBuilder UseWPF<TApplication>(this IPluginApplicationBuilder builder)
        where TApplication : Application
    {
        builder.InitializationServices.TryAddSingleton<IViewCollection, ViewCollection>();
        builder.InitializationServices.TryAddSingleton<Application, TApplication>();
        builder.InitializationServices.TryAddSingleton<IViewAdapterCollection, ViewAdapterCollection>();
        builder.InitializationServices.TryAddSingleton<IWPFThread, WPFThread>();

        builder.InitializationServices.AddStartupService<WPFAppInitializer>();

        builder.RuntimeServices.TryAddSingleton<IViewCompositionService, ViewCompositionService>();
        builder.RuntimeServices.TryAddSingleton<IViewAdapterCollection, ViewAdapterCollection>();
        builder.RuntimeServices.TryAddSingleton<IViewProvider, ViewProvider>();
        builder.RuntimeServices.TryAddSingleton<INavigationService, NavigationService>();
        builder.RuntimeServices.TryAddSingleton<IViewCompositionService, ViewCompositionService>();

        return builder;
    }

    public static IPluginApplicationBuilder UseSplashWindow<TViewModel>(this IPluginApplicationBuilder builder)
        where TViewModel : class, ISplashWindowViewModel
    {
        builder.InitializationServices.AddTransient<ISplashWindowViewModel, TViewModel>();
        return builder;
    }
}
