using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using ReactiveFramework.WPF.Internal;
using ReactiveFramework.WPF.ViewComposition;
using ReactiveFramework.Hosting;
using System.Windows;

namespace ReactiveFramework.WPF.Hosting;

internal class WPFAppBuilder : IWPFAppBuilder
{
    private readonly WPFAppOptions _options;
    private Type? _splashScreenType;
    private Type? _shellType;

    public WPFAppBuilder()
        : this(args: null)
    {
    }

    public WPFAppBuilder(string[]? args)
        : this(new WPFAppOptions
        {
            Args = args
        })
    {
    }

    public WPFAppBuilder(WPFAppOptions options)
    {
        _options = options;
    }

    protected virtual void ConfigureDefaultServices()
    {
        AppServices.TryAddSingleton(new WPFAppContext());
        AppServices.TryAddSingleton<IViewCollection, ViewCollection>();
        AppServices.TryAddSingleton<IViewProvider, ViewProvider>();
        AppServices.TryAddSingleton<Application, RxApp>();
        AppServices.TryAddSingleton<INavigationService, NavigationService>();
        AppServices.TryAddSingleton<IViewAdapterCollection, ViewAdapterCollection>();
        AppServices.TryAddSingleton<IViewCompositionService, ViewCompositionService>();

        AppServices.AddHostedService<HostedWPFAppService>();
    }
}