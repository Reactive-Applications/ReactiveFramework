using System.Windows;

namespace ReactiveFramework.WPF;
public interface ISplashWindowViewModel : IViewModel
{
    Type WindowType { get; }
    Window? SplashWindow { get; set; }
    Task OnAppStartAsync();
    Task AfterModuleInitalizationAsync();
}
