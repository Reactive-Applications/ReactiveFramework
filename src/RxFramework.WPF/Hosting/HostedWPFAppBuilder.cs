using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using RxFramework.Hosting;
using RxFramework.WPF.Internal;
using RxFramework.WPF.Theming;
using RxFramework.WPF.ViewComposition;
using System.Windows;

namespace RxFramework.WPF.Hosting;

internal class HostedWPFAppBuilder : HostedPluginAppBuilder, IHostedWPFAppBuilder
{
    private Type? _splashScreenType;
    private Type? _shellType;

    public HostedWPFAppBuilder()
        : this(args: null)
    {
    }

    public HostedWPFAppBuilder(string[]? args)
        : this(new WPFAppBuilderOptions
        {
            Args = args
        })
    {
    }

    public HostedWPFAppBuilder(WPFAppBuilderOptions options)
        : base(options)
    {
    }

    public IHostedWPFAppBuilder AddResources(string path)
    {
        void resDelegate(ResourceDictionary res) => res.MergedDictionaries.Add(new()
        {
            Source = new Uri("pack://application:,,,/" + path)
        });

        AppServices.AddSingleton(resDelegate);
        return this;
    }

    public IHostedWPFAppBuilder AddResources(ResourceDictionary resources)
    {
        void resDelegate(ResourceDictionary res) => res.MergedDictionaries.Add(resources);
        AppServices.AddSingleton(resDelegate);
        return this;
    }

    public IHostedWPFAppBuilder UseApp<TApp>() where TApp : Application
    {
        AppServices.AddSingleton<Application, TApp>();
        return this;
    }

    public IHostedWPFAppBuilder UseApp<TApp>(Func<IServiceProvider, TApp> appFactory) where TApp : Application
    {
        AppServices.AddSingleton<Application>(appFactory);
        return this;
    }

    public IHostedWPFAppBuilder UseShell<TShell>() where TShell : Window
    {
        _shellType = typeof(TShell);
        return this;
    }

    public IHostedWPFAppBuilder UseSplashScreen<TSplashScreen>() where TSplashScreen : Window
    {
        _splashScreenType = typeof(TSplashScreen);
        return this;
    }

    public override IHost Build()
    {
        ConfigureDefaultServices();

        var host = base.Build();

        RegisterSpecialViews(host.Services.GetRequiredService<IViewCollection>());

        return host;
    }

    protected virtual void ConfigureDefaultServices()
    {
        AppServices.TryAddSingleton(new WPFAppContext());
        AppServices.TryAddSingleton<IViewCollection, ViewCollection>();
        AppServices.TryAddSingleton<IViewProvider, ViewProvider>();
        AppServices.TryAddSingleton<Application, RxApp>();
        AppServices.TryAddSingleton<IThemeCollection, ThemeCollection>();
        AppServices.TryAddSingleton<IThemeManager, ThemeManager>();
        AppServices.TryAddSingleton<INavigationService, NavigationService>();
        AppServices.TryAddSingleton<IViewAdapterCollection, ViewAdapterCollection>();
        AppServices.TryAddSingleton<IViewCompositionService, ViewCompositionService>();

        AppServices.AddHostedService<HostedWPFAppService>();
    }

    protected virtual void RegisterSpecialViews(IViewCollection viewCollection)
    {
        if (_splashScreenType != null)
        {
            viewCollection.AddSplashScreen(_splashScreenType);
        }

        if (_shellType != null)
        {
            viewCollection.AddShell(_shellType);
        }
    }
}