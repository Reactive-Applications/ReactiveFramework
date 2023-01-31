using Microsoft.Extensions.DependencyInjection;
using RxFramework.Hosting.Plugins;
using RxFramework.WPF.ViewComposition;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace RxFramework.WPF;
public class RxApp : Application
{
    private readonly IViewProvider _viewProvider;
    private readonly IViewCollection _viewCollection;
    private readonly IPluginManager _pluginManager;
    private readonly IViewAdapterCollection _viewAdapters;

    new public static Dispatcher Dispatcher { get; internal set; } = Dispatcher.CurrentDispatcher;

    public IServiceProvider Services { get; }

    public RxApp(
        IServiceProvider services, 
        IViewProvider viewProvider, 
        IViewCollection viewCollection,
        IPluginManager pluginManager,
        IViewAdapterCollection viewAdapters)
    {
        Services = services;
        _viewProvider = viewProvider;
        _viewCollection = viewCollection;
        _pluginManager = pluginManager;
        _viewAdapters = viewAdapters;
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var splashWindow = ShowSplashScreen();
        SetupApp();
        await LoadAppFromSplashScreen(splashWindow);
        InitializePlugins();
        
        CreateShell();

        if (splashWindow != null)
        {
            splashWindow.Closed -= SplashWindowClosed;
        }

        splashWindow?.Close();
        ShowShell();
    }

    protected virtual Task LoadAppFromSplashScreen(Window? splashWindow)
    {
        if (splashWindow is null)
        {
            return Task.CompletedTask;
        }

        return splashWindow.DataContext is ISplashScreenViewModel vm ? vm.LoadAppAsync() : Task.CompletedTask;
    }

    protected virtual void InitializePlugins()
    {
        if (_pluginManager.AutoInitialize)
        {
            _pluginManager.InitializePlugins(Services);
        }
    }

    protected virtual void SetupApp()
    {
        RegisterViews();
        RegisterViewAdapters();
    }

    private void RegisterViews()
    {
        _viewCollection.AddViewsFromAssembly(Assembly.GetEntryAssembly()!);
    }

    private void RegisterViewAdapters()
    {
        _viewAdapters.AddAdaptersFromAssembly(Assembly.GetEntryAssembly()!);
        _viewAdapters.AddAdaptersFromAssembly(typeof(RxApp).Assembly);
    }

    protected virtual Window CreateShell()
    {
        var shell = _viewProvider.GetShell();
        MainWindow = shell;
        return shell;
    }

    protected virtual Window? ShowSplashScreen()
    {

        if (_viewProvider.IsSplashScreenRegistered())
        {
            return _viewProvider.GetSplashScreen();
        }
        return null;
    }

    protected virtual void ShowShell()
    {
        MainWindow.Show();
    }

    private void SplashWindowClosed(object? sender, EventArgs e)
    {
        Shutdown();
    }
}
