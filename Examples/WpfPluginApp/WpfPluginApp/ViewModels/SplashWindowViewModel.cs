using ReactiveFramework.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfPluginApp.Views;

namespace WpfPluginApp.ViewModels;
public class SplashWindowViewModel : ViewModelBase, ISplashWindowViewModel
{
    public Type WindowType => typeof(SplashWindow);

    public Task OnInitalizationAsync()
    {
        return Task.Delay(5000);
    }

    public Task OnAppStartAsync(IServiceProvider runtimeServices)
    {
        return Task.CompletedTask;
    }
}
