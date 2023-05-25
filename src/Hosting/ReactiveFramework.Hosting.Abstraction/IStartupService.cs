namespace ReactiveFramework.Modularity.Abstraction;
public interface IStartupService
{
    Task OnAppInitialization(IServiceProvider initializationServices, CancellationToken cancellationToken = default);

    Task OnAppStart(IServiceProvider runtimeServices, CancellationToken cancellationToken = default);
}
