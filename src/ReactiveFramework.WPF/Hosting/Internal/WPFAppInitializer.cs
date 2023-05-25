using Microsoft.Extensions.DependencyInjection;
using ReactiveFramework.Modularity.Abstraction;
using ReactiveFramework.WPF.ViewAdapters;
using ReactiveFramework.WPF.ViewComposition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ReactiveFramework.WPF.Hosting.Internal;
public class WPFAppInitializer : IStartupService
{
    private Window? _splashWindow;
    private ISplashWindowViewModel? _splashViewModel;

    public async Task OnAppInitialization(IServiceProvider initializationServices, CancellationToken cancellationToken = default)
    {
        var runtimeServices = initializationServices.GetRequiredService<IServiceCollection>();
        runtimeServices.AddSingleton(initializationServices.GetRequiredService<IViewCollection>());
        runtimeServices.AddSingleton(initializationServices.GetRequiredService<IViewAdapterCollection>());

        var wpfThread = initializationServices.GetRequiredService<IWPFThread>();

        await wpfThread.WaitForAppBuild();

        if (!wpfThread.AppCreated)
        {
            throw new InvalidOperationException("wpf app was not created succsesfully");
        }
        runtimeServices.AddSingleton(wpfThread.Application);

        runtimeServices.AddSingleton(wpfThread);
        wpfThread.StartApplication();
        _splashViewModel = initializationServices.GetService<ISplashWindowViewModel>();
        if (_splashViewModel != null)
        {
            await wpfThread.UiDispatcher.InvokeAsync(() =>
            {
                _splashWindow = Activator.CreateInstance(_splashViewModel.WindowType) as Window ??
                    throw new NotSupportedException("the provided splashWindowType is not valid. The splash window must be an Window and " +
                    "it must contain a parameterless constructor");

                _splashWindow.DataContext = _splashViewModel;

                wpfThread.Application.MainWindow = _splashWindow;
                _splashWindow.Show();
            });
            await _splashViewModel.OnInitalizationAsync();
        }
        
        var viewCollection = initializationServices.GetRequiredService<IViewCollection>();
        var viewAdapters = initializationServices.GetRequiredService<IViewAdapterCollection>();
        var executingAssembly = Assembly.GetEntryAssembly()!;

        viewCollection.AddViewsFromAssembly(executingAssembly);
        viewAdapters.AddAdaptersFromAssembly(executingAssembly);
        viewAdapters.AddAdaptersFromAssembly(Assembly.GetAssembly(typeof(ContentControlAdapter))!);

        return;
    }

    public async Task OnAppStart(IServiceProvider runtimeServices, CancellationToken cancellationToken = default)
    {
        var wpfThread = runtimeServices.GetRequiredService<IWPFThread>();

        if (!wpfThread.AppCreated)
        {
            throw new InvalidOperationException("wpf app was not created succsesfully");
        }

        if (!wpfThread.AppIsRunnning)
        {
            wpfThread.StartApplication();
        }

        if(_splashViewModel is not null)
        {
            await _splashViewModel.OnAppStartAsync(runtimeServices);
        }

        await wpfThread.UiDispatcher.InvokeAsync(() =>
        {
            var viewProvider = runtimeServices.GetRequiredService<IViewProvider>();
            var shell = viewProvider.GetShell();
            wpfThread.Application.MainWindow = shell;

            shell.Show();
            _splashWindow?.Close();
        });

        return;
    }
}
