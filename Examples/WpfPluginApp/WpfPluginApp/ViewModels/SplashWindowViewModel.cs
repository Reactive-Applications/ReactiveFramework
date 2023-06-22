using ReactiveFramework.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfPluginApp.Views;

namespace WpfPluginApp.ViewModels;
public class SplashWindowViewModel : ViewModelBase, ISplashWindowViewModel
{
    public Type WindowType => typeof(SplashWindow);

    public Window? SplashWindow { get; set; }

    public async Task AfterModuleInitalizationAsync()
    {
        await Task.Delay(3000);
    }

    public Task OnAppStartAsync()
    {
        return Task.CompletedTask;
    }
}
