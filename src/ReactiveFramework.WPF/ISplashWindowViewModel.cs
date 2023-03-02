namespace ReactiveFramework.WPF;
public interface ISplashWindowViewModel : IViewModel
{
    Type WindowType { get; }
    Task OnInitalizationAsync();
    Task OnAppStartAsync(IServiceProvider runtimeServices);
}
